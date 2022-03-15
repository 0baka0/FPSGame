using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    public KeyCode keyCodeRun = KeyCode.LeftShift; // �޸��� Ű

    [Header("Audio Clips")]
    public AudioClip audioClipWalk;     // �ȱ� ����
    public AudioClip audioClipRun;      // �޸��� ����

    private RotateToMouse rotateToMouse;    // ���콺 �̵����� ī�޶� ȸ��
    private PlayerMovement movement;        // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status status;                  // �̵��ӵ� ���� ĳ���� ����
    private PlayerAnim animator;            // �ִϸ��̼� ��� ����
    private AudioSource audioSource;        // ���� ��� ����

    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� �����ϰ�, ���� ��ġ�� ������Ų��.
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();  // ��ũ��Ʈ�� ������Ʈ�� �����´�.
        movement = GetComponent<PlayerMovement>();      // ��ũ��Ʈ�� ������Ʈ�� �����´�.
        status = GetComponent<Status>();                // ��ũ��Ʈ�� ������Ʈ�� �����´�.
        animator = GetComponent<PlayerAnim>();          // ��ũ��Ʈ�� ������Ʈ�� �����´�.
        audioSource = GetComponent<AudioSource>();      // AudioSource ������Ʈ�� �����´�.
    }

    private void Update()
    {
        // ȣ��
        UpdateRotate();
        UpdateMove();
    }

    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdtaeRotate(mouseX, mouseY);
    }

    private void UpdateMove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // �̵� ���� �� (�ȱ� or �ٱ�)
        if(x != 0 || z != 0)
        {
            bool isRun = false;

            // ���̳� �ڷ� �̵��� ���� �޸� �� ����
            if (z > 0) isRun = Input.GetKey(keyCodeRun);

            
            Debug.Log(isRun);
            movement.MoveSpeed = isRun == true ? status.runSpeed : status.walkSpeed;
            animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            Debug.Log(isRun == true ? audioClipRun : audioClipWalk);
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            // ����Ű �Է� ���δ� �� ������ Ȯ���ϱ� ������
            // ��� ���϶��� �ٽ� ������� �ʵ��� isPlaying���� üũ�ؼ� ���
            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }

        }
        // ���ڸ��� �������� ��
        else
        {
            movement.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            // ������ �� ���尡 ������̸� ����
            if(audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        movement.MoveTo(new Vector3(x, 0, z));
    }
}
