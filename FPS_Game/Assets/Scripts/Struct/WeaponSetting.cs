using System;

[Serializable] // ����ȭ
public struct WeaponSetting // ����ü�� ���ÿ���, Ŭ������ �� ������ �޸� �Ҵ�ȴ�.
{
    public float attackRate;        // ���� �ӵ�
    public float attackDistance;    // ���� ��Ÿ�
    public bool isAutomaticAttack;  // ���� ���� ����
}
