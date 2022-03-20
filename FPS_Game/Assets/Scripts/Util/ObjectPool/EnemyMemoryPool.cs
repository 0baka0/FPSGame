using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    public Transform target;                                // 적의 목표 (플레이어)
    public GameObject enemySpawnPointPrefab;                // 적이 등장하기 전 적의 등장 위치를 알려주는 프리팹
    public GameObject enemyPrefab;                          // 생성되는 적 프리팹
    public float enemySpawnTime = 1;                        // 적 생성 주기
    public float enemySpawnLatency;                         // 타일 생성 후 적이 등장하기까지 대기 시간
                                                            
    private MemoryPool spawnPointMemoryPool;                // 적 등장 위치를 알려주는 오브젝트 생성, 활성/비활성 관리
    private MemoryPool enemyMemoryPool;                     // 적 생성, 활성/비활성 관리
                                                            
    private int numberEnemiesSpawnedAtOnce = 1;             // 동시에 생성되는 적의 숫자
    private Vector2Int mapSize = new Vector2Int(100, 100);  // 맵 크기

    private void Awake()
    {
        spawnPointMemoryPool = new MemoryPool(enemySpawnPointPrefab);
        enemyMemoryPool = new MemoryPool(enemyPrefab);

        StartCoroutine("SpawnTile");
    }

    private IEnumerator SpawnTile()
    {
        int currentNumber = 0;
        int maximumNumber = 50;

        while (true)
        {
            for (int i = 0; i < numberEnemiesSpawnedAtOnce; i++)
            {
                GameObject item = spawnPointMemoryPool.ActivatePoolItem();

                item.transform.position = new Vector3(Random.Range(-mapSize.x * .49f, mapSize.x * .49f), 1,
                                                      Random.Range(-mapSize.y * .49f, mapSize.y * .49f));

                StartCoroutine("SpawnEnemy", item);
            }

            currentNumber++;

            if(currentNumber >= maximumNumber)
            {
                currentNumber = 0;
                numberEnemiesSpawnedAtOnce++;
            }

            yield return new WaitForSeconds(enemySpawnTime);
        }
    }

    private IEnumerator SpawnEnemy(GameObject point)
    {
        yield return new WaitForSeconds(enemySpawnLatency);

        // 적 오브젝트를 생성하고, 적의 위치를 point의 위치로 설정
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        // EnemyFSM.Setup 매개변수로 target을 전달
        item.GetComponent<EnemyFSM>().Setup(target);

        // 타일 오브젝트를 비활성화
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }
}
