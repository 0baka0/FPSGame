using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private MovementTransform movement;
    private float projectileDistance = 30;  // 발사체 최대 발사거리
    public int damage = 5;                  // 발사체 공격력

    public void Setup(Vector3 position)
    {
        movement = GetComponent<MovementTransform>();

        StartCoroutine("OnMove", position);
    }

    // 이동 방향 설정과 이동 범위 초과 여부 확인
    private IEnumerator OnMove(Vector3 targetPosition)
    {
        Vector3 start = transform.position;

        movement.MoveTo((targetPosition - transform.position).normalized);

        while (true)
        {
            if(Vector3.Distance(transform.position, start) >= projectileDistance)
            {
                Destroy(gameObject);

                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("Enemy : 때림");
            other.GetComponent<PlayerController>().TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
