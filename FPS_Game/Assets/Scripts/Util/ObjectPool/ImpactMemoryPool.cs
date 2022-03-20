using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 벽과 바닥은 일반 타격, 장애물은 장애물 타격으로 하기 위해 열거형으로 생성
public enum ImpactType { Normal = 0, Obstacle, }

public class ImpactMemoryPool : MonoBehaviour
{
    public GameObject[] impactPrefabs;  // 피격 이펙트
    private MemoryPool[] memoryPools;   // 피격 이펙트 메모리풀

    // 메모리풀의 배열 변수 개수를 설정하고 메모리풀 배열의 각 방에 ImpactPrefabObject를 등록한다
    private void Awake()
    {
        // 피격 이벤트가 여러 종류이면 종류별로 memoryPool 생성
        memoryPools = new MemoryPool[impactPrefabs.Length];
        for (int i = 0; i < impactPrefabs.Length; ++i)
        {
            memoryPools[i] = new MemoryPool(impactPrefabs[i]);
        }
    }

    // 매개변수로 받아올 RacastHit를 바탕으로 부딪힌 오브젝트 tag 정보를 분석해 피격 이펙트를 생성한다
    public void SpawnImpact(RaycastHit hit)
    {
        // 부딪힌 오브젝트의 tag 정보에 따라 다르게 처리
        if(hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    // 피격이펙트 타입과, 생성위치, 회전 매개변수를 바탕으로 피격이펙트를 생성하고
    // 위치, 회전값을 설정하고 Impact.Setup 메서드를 호출한다
    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPools[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPools[(int)type]);
    }
}
