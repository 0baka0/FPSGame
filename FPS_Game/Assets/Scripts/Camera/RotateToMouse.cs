using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    public float rotCamXAxisSpeed = 5;  // 카메라 x축 회전 속도
    public float rotCamYAxisSpeed = 3;  // 카메라 y축 회전 속도

    private float limitMinX = -80;      // 카메라 x축 회전 범위 (최소)
    private float limitMaxX = 50;       // 카메라 x축 회전 범위 (최대)
    private float eulerAngleX;          // 위/아래 이동
    private float eulerAngleY;          // 좌/우 이동

    // 카메라 회전을 제어할 때 호출하는 업데이트
    /// 마우스를 좌/우로 움직였을 떄 오브젝트가 실제 회전하는 축은 y축
    /// 마우스를 위/아래로 움직였을 때 오브젝트가 실제 회전하는 축은 x축
    /// 그렇기 때문에 mouseX값을 eulerAngleY값에 적용(mouseY는 eulerAngleX)
    /// 마우스를 아래로 내리면 -로 음수인데 오브젝트의 x축이 +방향으로
    /// 회전해야 아래를 보기 때문에 18줄은 eulerAngleX -=..으로 설정
    public void UpdtaeRotate(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * rotCamYAxisSpeed; // 마우스 좌/우 이동으로 카메라 y축 회전
        eulerAngleX -= mouseY * rotCamXAxisSpeed; // 마우스 위/아래 이동으로 카메라 x축 회전

        // 카메라 x축 회전의 경우 회전 범위를 설정
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
