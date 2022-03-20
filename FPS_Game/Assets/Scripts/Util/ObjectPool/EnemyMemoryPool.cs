using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMemoryPool : MonoBehaviour
{
    public Transform target;                                // ���� ��ǥ (�÷��̾�)
    public GameObject enemySpawnPointPrefab;                // ���� �����ϱ� �� ���� ���� ��ġ�� �˷��ִ� ������
    public GameObject enemyPrefab;                          // �����Ǵ� �� ������
    public float enemySpawnTime = 1;                        // �� ���� �ֱ�
    public float enemySpawnLatency;                         // Ÿ�� ���� �� ���� �����ϱ���� ��� �ð�
                                                            
    private MemoryPool spawnPointMemoryPool;                // �� ���� ��ġ�� �˷��ִ� ������Ʈ ����, Ȱ��/��Ȱ�� ����
    private MemoryPool enemyMemoryPool;                     // �� ����, Ȱ��/��Ȱ�� ����
                                                            
    private int numberEnemiesSpawnedAtOnce = 1;             // ���ÿ� �����Ǵ� ���� ����
    private Vector2Int mapSize = new Vector2Int(100, 100);  // �� ũ��

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

        // �� ������Ʈ�� �����ϰ�, ���� ��ġ�� point�� ��ġ�� ����
        GameObject item = enemyMemoryPool.ActivatePoolItem();
        item.transform.position = point.transform.position;

        // EnemyFSM.Setup �Ű������� target�� ����
        item.GetComponent<EnemyFSM>().Setup(target);

        // Ÿ�� ������Ʈ�� ��Ȱ��ȭ
        spawnPointMemoryPool.DeactivatePoolItem(point);
    }
}
