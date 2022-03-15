using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;  // �̵� �ӵ�
    private Vector3 moveForce;  // �̵� �� (x, z�� y���� ������ ����� ���� �̵��� ����)

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0, value);
    }

    private CharacterController characterController;    // �÷��̾� �̵� ��� ���� ������Ʈ

    private void Awake()
    {
        // CharacterController ������Ʈ�� �����´�.
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // �ʴ� moveForce �ӷ����� �̵�
        characterController.Move(moveForce * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        // �̵� ���� = ĳ������ ȸ�� �� * ���� ��
        direction = transform.rotation * new Vector3(direction.x, 0, direction.z);

        // �̵� �� = �̵����� * �ӵ�
        moveForce = new Vector3(direction.x * moveSpeed, moveForce.y, direction.z * moveSpeed);
    }
}
