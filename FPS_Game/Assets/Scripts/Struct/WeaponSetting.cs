using System;

[Serializable] // 직렬화
public struct WeaponSetting // 구조체는 스택영역, 클래스는 힙 영역에 메모리 할당된다.
{
    public float attackRate;        // 공격 속도
    public float attackDistance;    // 공격 사거리
    public bool isAutomaticAttack;  // 연속 공격 여부
}
