using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    public KeyCode keyCodeRun = KeyCode.LeftShift;  // 달리기 키
    public KeyCode keyCodeJump = KeyCode.Space;     // 점프 키
    public KeyCode keyCodeReload = KeyCode.R;       // 탄 재장전 키

    [Header("Audio Clips")]
    public AudioClip audioClipWalk;                 // 걷기 사운드
    public AudioClip audioClipRun;                  // 달리기 사운드

    private RotateToMouse rotateToMouse;            // 마우스 이동으로 카메라 회전
    private PlayerMovement movement;                // 키보드 입력으로 플레이어 이동, 점프
    private Status status;                          // 이동속도 등의 캐릭터 정보
    private PlayerAnim animator;                    // 애니메이션 재생 제어
    private AudioSource audioSource;                // 사운드 재생 제어
    private WeaponAssaultRifle weapon;              // 무기를 이용한 공격 제어

    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정하고, 현재 위치에 고정시킨다.
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();  // RotateToMouse스크립트 컴포넌트를 가져온다
        movement = GetComponent<PlayerMovement>();      // PlayerMovement스크립트 컴포넌트를 가져온다
        status = GetComponent<Status>();                // Status스크립트 컴포넌트를 가져온다
        animator = GetComponent<PlayerAnim>();          // PlayerAnim스크립트 컴포넌트를 가져온다
        audioSource = GetComponent<AudioSource>();      // AudioSource 컴포넌트를 가져온다
        // "Player" 오브젝트 기준으로 자식 오브젝트인
        // "arms_assault-rifle_01" 오브젝트에 Animator 컴포넌트가 있으니
        // GetComponent 아닌 GetComponentInChildren를 사용한다
        weapon = GetComponentInChildren<WeaponAssaultRifle>();
    }

    private void Update()
    {
        // 호출
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

        // 이동 중일 때 (걷기 or 뛰기)
        if(x != 0 || z != 0)
        {
            bool isRun = false;

            // 옆이나 뒤로 이동할 때는 달릴 수 없다
            if (z > 0) isRun = Input.GetKey(keyCodeRun);

            
            Debug.Log(isRun);
            movement.MoveSpeed = isRun == true ? status.runSpeed : status.walkSpeed;
            animator.MoveSpeed = isRun == true ? 1 : 0.5f;
            Debug.Log(isRun == true ? audioClipRun : audioClipWalk);
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            // 방향키 입력 여부는 매 프레임 확인하기 때문에
            // 재생 중일때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }

        }
        // 제자리에 멈춰있을 때
        else
        {
            movement.MoveSpeed = 0;
            animator.MoveSpeed = 0;

            // 멈췄을 때 사운드가 재생중이면 정지
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
        // 마우스 왼쪽 버튼을 눌렀을 때
        if(Input.GetMouseButtonDown(0))
        {
            // 공격 시작
            weapon.StartWeaponAction();
        }
        // 마우스 왼쪽 버튼을 뗐을 때
        else if(Input.GetMouseButtonUp(0))
        {
            // 공격 종료
            weapon.StopWeaponAction();
        }

        // 마우스 오른쪽 버튼을 눌렀을 때
        if(Input.GetMouseButtonDown(1))
        {
            // 모드 전환
            weapon.StartWeaponAction(1);
        }
        // 마우스 오른쪽 버튼을 뗐을 때
        else if(Input.GetMouseButtonUp(1))
        {
            // 별다른 호출은 없음
            weapon.StopWeaponAction(1);
        }

        // R 버튼을 눌렀을 때
        if(Input.GetKeyDown(keyCodeReload))
        {
            // 장전
            weapon.StartReload();
        }
    }

    public void TakeDamage(int damage)
    {
        bool isDie = status.DecreaseHP(damage);

        if(isDie == true)
        {
            Debug.Log("게임 종료");
        }
    }
}
