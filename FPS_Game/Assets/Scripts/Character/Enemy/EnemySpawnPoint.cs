using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public float fadeSpeed = 4;         // 점차 안보이게 만들 fade값
    private MeshRenderer meshRenderer;  // 오브젝트의 컬러 값을 이용할 MeshRenderer

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();    // MeshRenderer 컴포넌트를 가져온다
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
