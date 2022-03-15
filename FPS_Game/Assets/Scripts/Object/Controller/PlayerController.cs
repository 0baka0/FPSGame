using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Input KeyCodes")]
    public KeyCode keyCodeRun = KeyCode.LeftShift; // 달리기 키

    [Header("Audio Clips")]
    public AudioClip audioClipWalk;     // 걷기 사운드
    public AudioClip audioClipRun;      // 달리기 사운드

    private RotateToMouse rotateToMouse;    // 마우스 이동으로 카메라 회전
    private PlayerMovement movement;        // 키보드 입력으로 플레이어 이동, 점프
    private Status status;                  // 이동속도 등의 캐릭터 정보
    private PlayerAnim animator;            // 애니메이션 재생 제어
    private AudioSource audioSource;        // 사운드 재생 제어

    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정하고, 현재 위치에 고정시킨다.
        Cursor.visible      = false;
        Cursor.lockState    = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();  // 스크립트의 컴포넌트를 가져온다.
        movement = GetComponent<PlayerMovement>();      // 스크립트의 컴포넌트를 가져온다.
        status = GetComponent<Status>();                // 스크립트의 컴포넌트를 가져온다.
        animator = GetComponent<PlayerAnim>();          // 스크립트의 컴포넌트를 가져온다.
        audioSource = GetComponent<AudioSource>();      // AudioSource 컴포넌트를 가져온다.
    }

    private void Update()
    {
        // 호출
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
}
