using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    public WeaponAssaultRifle weapon;       // 현재 정보가 출력되는 무시
                                            
    [Header("Weapon Base")]                 
    public TextMeshProUGUI textWeaponName;  // 무기 이름
    public Image imageWeaponIcon;           // 무기 아이콘
    public Sprite[] spriteWeaponIcons;      // 무기 아이콘에 사용되는 sprite 배열
                                            
    [Header("Ammo")]                        
    public TextMeshProUGUI textAmmo;        // 현재/최대 탄 수 출력 Text

    [Header("Magazine")]
    public GameObject magazineUIPrefab;     // 탄창 UI 프리팹
    public Transform magazineParent;        // 탄창 UI가 배치되는 Panel

    private List<GameObject> magazineList;  // 탄창 UI 리스트

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();

        // 메소드가 등록되어 있는 이벤트 클래스(weapon.xx)의
        // Invoke() 메소드가 호출될 때 등록된 메소드(매개변수)가 실행된다
        weapon.onAmmoEvent.AddListener(UpdateAmmoHUD);
        weapon.onMagazineEvent.AddListener(UpdateMagazineHUD);
    }

    private void SetupWeapon()
    {
        textWeaponName.text = weapon.WeaponName.ToString();
        imageWeaponIcon.sprite = spriteWeaponIcons[(int)weapon.WeaponName];
    }

    private void UpdateAmmoHUD(int currentAmmo, int maxAmmo)
    {
        textAmmo.text = $"<size=40>{currentAmmo}/</size>{maxAmmo}";
    }

    private void SetupMagazine()
    {
        // weapon에 등록되어 있느 ㄴ최대 탄창 개수만큼 Image Icon을 생성
        // magazineParent 오브젝트의 자식으로 등록 후 모두 비활성화/리스트에 저장
        /// UI에 표현될 수 있는 최대 개수(=maxMagazine)만큼 생성해서
        /// 현재 탄창 수만큼 활성화해서 사용
        magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }

        // weapon에 등록되어 있는 탄창 개수만큼 오브젝트 활성화
        for (int i = 0; i < weapon.CurrentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        // 전부 비활성화하고, currentMagazine 개수만큼 활성화
        for (int i = 0; i < magazineList.Count; ++i)
        {
            magazineList[i].SetActive(false);
        }
        for (int i = 0; i < currentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }


}
