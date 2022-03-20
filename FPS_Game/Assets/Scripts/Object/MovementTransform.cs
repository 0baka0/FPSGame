using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTransform : MonoBehaviour
{
    public float moveSpeed = .0f;
    public Vector3 moveDirection = Vector3.zero;

    // �̵� ������ �����Ǹ� �̵��ϵ��� ��
    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    // �ܺο��� �Ű������� �̵� ������ ����
    public void MoveTo(Vector3 direction)
    {
        moveDirection = direction;
    }
}
