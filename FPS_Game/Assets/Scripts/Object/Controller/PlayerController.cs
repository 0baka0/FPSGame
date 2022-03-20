using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    public KeyCode keyCodeRun = KeyCode.LeftShift;  // �޸��� Ű
    public KeyCode keyCodeJump = KeyCode.Space;     // ���� Ű
    public KeyCode keyCodeReload = KeyCode.R;       // ź ������ Ű

    [Header("Audio Clips")]
    public AudioClip audioClipWalk;                 // �ȱ� ����
    public AudioClip audioClipRun;                  // �޸��� ����

    private RotateToMouse rotateToMouse;            // ���콺 �̵����� ī�޶� ȸ��
    private PlayerMovement movement;                // Ű���� �Է����� �÷��̾� �̵�, ����
    private Status status;                          // �̵��ӵ� ���� ĳ���� ����
    private PlayerAnim animator;                    // �ִϸ��̼� ��� ����
    private AudioSource audioSource;                // ���� ��� ����
    private WeaponAssaultRifle weapon;              // ���⸦ �̿��� ���� ����

    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� �����ϰ�, ���� ��ġ�� ������Ų��.
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();  // RotateToMouse��ũ��Ʈ ������Ʈ�� �����´�
        movement = GetComponent<PlayerMovement>();      // PlayerMovement��ũ��Ʈ ������Ʈ�� �����´�
        status = GetComponent<Status>();                // Status��ũ��Ʈ ������Ʈ�� �����´�
        animator = GetComponent<PlayerAnim>();          // PlayerAnim��ũ��Ʈ ������Ʈ�� �����´�
        audioSource = GetComponent<AudioSource>();      // AudioSource ������Ʈ�� �����´�
        // "Player" ������Ʈ �������� �ڽ� ������Ʈ��
        // "arms_assault-rifle_01" ������Ʈ�� Animator ������Ʈ�� ������
        // GetComponent �ƴ� GetComponentInChildren�� ����Ѵ�
        weapon = GetComponentInChildren<WeaponAssaultRifle>();
    }

    private void Update()
    {
        // ȣ��
        UpdateRotate();
        UpdateMove();
        UpdateJump();
        UpdateWeaponAction();
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

    private void UpdateJump()
    {
        if(Input.GetKeyDown(keyCodeJump))
        {
            movement.Jump();
        }
    }

    private void UpdateWeaponAction()
    {
        // ���콺 ���� ��ư�� ������ ��
        if(Input.GetMouseButtonDown(0))
        {
            // ���� ����
            weapon.StartWeaponAction();
        }
        // ���콺 ���� ��ư�� ���� ��
        else if(Input.GetMouseButtonUp(0))
        {
            // ���� ����
            weapon.StopWeaponAction();
        }

        // ���콺 ������ ��ư�� ������ ��
        if(Input.GetMouseButtonDown(1))
        {
            // ��� ��ȯ
            weapon.StartWeaponAction(1);
        }
        // ���콺 ������ ��ư�� ���� ��
        else if(Input.GetMouseButtonUp(1))
        {
            // ���ٸ� ȣ���� ����
            weapon.StopWeaponAction(1);
        }

        // R ��ư�� ������ ��
        if(Input.GetKeyDown(keyCodeReload))
        {
            // ����
            weapon.StartReload();
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie == true)
        {
            Debug.Log("���� ����");
        }
    }
}
