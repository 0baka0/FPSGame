using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impact : MonoBehaviour
{
    private ParticleSystem particle;
    private MemoryPool memoryPool;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>(); // ParticleSystem 컴포넌트를 가져온다
    }

    // 타격 이펙트는 삭제하지 않고 메모리풀로 관리하기 때문에 셋업 메서드에서 
    // 메모리풀 매개변수를 받아와 멤버변수 메모리풀에 저장한다
    public void Setup(MemoryPool pool)
    {
        memoryPool = pool;
    }

    private void Update()
    {
        // 파티클이 재생중이 아니면 삭제
        if(particle.isPlaying == false)
        {
            memoryPool.DeactivatePoolItem(gameObject);
        }
    }
}
