using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casing : MonoBehaviour
{
    public float deactivateTime = 5.0f;     // ź�� ���� �� ��Ȱ��ȭ �Ǵ� �ð�
    public float casingSpin = 1.0f;         // ź�ǰ� ȸ���ϴ� �ӷ� ����
    public AudioClip[] audioClips;          // ź�ǰ� �ε����� �� ����Ǵ� ����

    private Rigidbody rigidbody3D;          //
    private AudioSource audioSource;        //
    private MemoryPool memoryPool;          // 

    public void Setup(MemoryPool pool, Vector3 direction)
    {
        rigidbody3D = GetComponent<Rigidbody>();    // Rigidbody ������Ʈ�� �����´�
        audioSource = GetComponent<AudioSource>();  // AudioSource ������Ʈ�� �����´�
        memoryPool = pool;

        // ź���� �̵� �ӷ°� ȸ�� �ӷ� ����
        rigidbody3D.velocity = new Vector3(direction.x, 1.0f, direction.z);
        rigidbody3D.angularVelocity = new Vector3(Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin),
                                                  Random.Range(-casingSpin, casingSpin));
        StartCoroutine("DeactivateAfterTime");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �������� ź�� ���� �� ������ ���� ����
        int index = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(deactivateTime);

        memoryPool.DeactivatePoolItem(this.gameObject);
    }
}
