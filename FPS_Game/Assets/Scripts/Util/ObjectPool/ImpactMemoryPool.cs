using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ٴ��� �Ϲ� Ÿ��, ��ֹ��� ��ֹ� Ÿ������ �ϱ� ���� ���������� ����
public enum ImpactType { Normal = 0, Obstacle, }

public class ImpactMemoryPool : MonoBehaviour
{
    public GameObject[] impactPrefabs;  // �ǰ� ����Ʈ
    private MemoryPool[] memoryPools;   // �ǰ� ����Ʈ �޸�Ǯ

    // �޸�Ǯ�� �迭 ���� ������ �����ϰ� �޸�Ǯ �迭�� �� �濡 ImpactPrefabObject�� ����Ѵ�
    private void Awake()
    {
        // �ǰ� �̺�Ʈ�� ���� �����̸� �������� memoryPool ����
        memoryPools = new MemoryPool[impactPrefabs.Length];
        for (int i = 0; i < impactPrefabs.Length; ++i)
        {
            memoryPools[i] = new MemoryPool(impactPrefabs[i]);
        }
    }

    // �Ű������� �޾ƿ� RacastHit�� �������� �ε��� ������Ʈ tag ������ �м��� �ǰ� ����Ʈ�� �����Ѵ�
    public void SpawnImpact(RaycastHit hit)
    {
        // �ε��� ������Ʈ�� tag ������ ���� �ٸ��� ó��
        if(hit.transform.CompareTag("ImpactNormal"))
        {
            OnSpawnImpact(ImpactType.Normal, hit.point, Quaternion.LookRotation(hit.normal));
        }
        else if(hit.transform.CompareTag("ImpactObstacle"))
        {
            OnSpawnImpact(ImpactType.Obstacle, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }

    // �ǰ�����Ʈ Ÿ�԰�, ������ġ, ȸ�� �Ű������� �������� �ǰ�����Ʈ�� �����ϰ�
    // ��ġ, ȸ������ �����ϰ� Impact.Setup �޼��带 ȣ���Ѵ�
    public void OnSpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        GameObject item = memoryPools[(int)type].ActivatePoolItem();
        item.transform.position = position;
        item.transform.rotation = rotation;
        item.GetComponent<Impact>().Setup(memoryPools[(int)type]);
    }
}
