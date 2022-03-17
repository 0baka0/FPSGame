using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [Header("Components")]
    public WeaponAssaultRifle weapon;       // ���� ������ ��µǴ� ����
                                            
    [Header("Weapon Base")]                 
    public TextMeshProUGUI textWeaponName;  // ���� �̸�
    public Image imageWeaponIcon;           // ���� ������
    public Sprite[] spriteWeaponIcons;      // ���� �����ܿ� ���Ǵ� sprite �迭
                                            
    [Header("Ammo")]                        
    public TextMeshProUGUI textAmmo;        // ����/�ִ� ź �� ��� Text

    [Header("Magazine")]
    public GameObject magazineUIPrefab;     // źâ UI ������
    public Transform magazineParent;        // źâ UI�� ��ġ�Ǵ� Panel

    private List<GameObject> magazineList;  // źâ UI ����Ʈ

    private void Awake()
    {
        SetupWeapon();
        SetupMagazine();

        // �޼ҵ尡 ��ϵǾ� �ִ� �̺�Ʈ Ŭ����(weapon.xx)��
        // Invoke() �޼ҵ尡 ȣ��� �� ��ϵ� �޼ҵ�(�Ű�����)�� ����ȴ�
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
        // weapon�� ��ϵǾ� �ִ� ���ִ� źâ ������ŭ Image Icon�� ����
        // magazineParent ������Ʈ�� �ڽ����� ��� �� ��� ��Ȱ��ȭ/����Ʈ�� ����
        /// UI�� ǥ���� �� �ִ� �ִ� ����(=maxMagazine)��ŭ �����ؼ�
        /// ���� źâ ����ŭ Ȱ��ȭ�ؼ� ���
        magazineList = new List<GameObject>();
        for (int i = 0; i < weapon.MaxMagazine; ++i)
        {
            GameObject clone = Instantiate(magazineUIPrefab);
            clone.transform.SetParent(magazineParent);
            clone.SetActive(false);

            magazineList.Add(clone);
        }

        // weapon�� ��ϵǾ� �ִ� źâ ������ŭ ������Ʈ Ȱ��ȭ
        for (int i = 0; i < weapon.CurrentMagazine; ++i)
        {
            magazineList[i].SetActive(true);
        }
    }

    private void UpdateMagazineHUD(int currentMagazine)
    {
        // ���� ��Ȱ��ȭ�ϰ�, currentMagazine ������ŭ Ȱ��ȭ
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
