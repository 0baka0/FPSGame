using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public float fadeSpeed = 4;         // ���� �Ⱥ��̰� ���� fade��
    private MeshRenderer meshRenderer;  // ������Ʈ�� �÷� ���� �̿��� MeshRenderer

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();    // MeshRenderer ������Ʈ�� �����´�
    }

    private void OnEnable()
    {
        StartCoroutine("OnFadeEffect");
    }

    private void OnDisable()
    {
        StopCoroutine("OnFadeEffect");
    }

    private IEnumerator OnFadeEffect()
    {
        while(true)
        {
            Color color = meshRenderer.material.color;
            color.a = Mathf.Lerp(1, 0, Mathf.PingPong(Time.time * fadeSpeed, 1));
            meshRenderer.material.color = color;

            yield return null;
        }
    }
}
