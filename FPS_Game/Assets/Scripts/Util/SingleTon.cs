using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : SingleTon<T>
{
    // 싱글턴에서 파생된 클래스의 인스턴스
    private static T instance = null;

    // 외부에서 싱글턴 인스턴스에 접근하기 위한 읽기 전용 프로퍼티
    //  -> get에서 instance가 존재하는지 확인
    public static T Instance
    {
        get
        {
            // 인스턴스가 존재하는지 체크
            if (instance == null)
            {
                // 인스턴스를 씬에서 찾음
                instance = FindObjectOfType<T>();
                // 인스턴스를 찾았는지 체크
                if (instance == null)
                {
                    // 없다면 게임 오브젝트를 생성
                    GameObject obj = new GameObject(typeof(T).Name);
                    // 생성한 빈 객체에 T타입 컴포넌트를 붙인다
                    obj.AddComponent<T>();
                }
            }

            
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // 인스턴스가 없다면
        if(instance == null)
        {
            // 인스턴스를 미리 넣어주는 작업
            //  -> Instance 프로퍼티를 통해 접근 시 객체를 찾거나 생성하는 과정을 생략
            instance = this as T;
            // 씬이 변경되도 게임 오브젝트가 파괴되지 않도록
            DontDestroyOnLoad(gameObject);
        }
        // 인스턴스가 있다면
        else
        {
            // 이 시점에서 인스턴스가 존재하는 것은 잘못된 사용으로 인한 복수의 인스턴스 생성
            Destroy(gameObject);
        }
    }


}
