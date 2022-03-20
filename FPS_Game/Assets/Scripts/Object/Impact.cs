using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>(); // ParticleSystem ������Ʈ�� �����´�
    }

    // Ÿ�� ����Ʈ�� �������� �ʰ� �޸�Ǯ�� �����ϱ� ������ �¾� �޼��忡�� 
    // �޸�Ǯ �Ű������� �޾ƿ� ������� �޸�Ǯ�� �����Ѵ�
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    private void Update()
    {
        // ��ƼŬ�� ������� �ƴϸ� ����
        if(particle.isPlaying == false)
        {
            memoryPool.DeactivatePoolItem(gameObject);
        }
    }
}
