using UnityEngine;

public class SingleTon<T> : MonoBehaviour where T : SingleTon<T>
{
    // �̱��Ͽ��� �Ļ��� Ŭ������ �ν��Ͻ�
    private static T instance = null;

    // �ܺο��� �̱��� �ν��Ͻ��� �����ϱ� ���� �б� ���� ������Ƽ
    //  -> get���� instance�� �����ϴ��� Ȯ��
    public static T Instance
    {
        get
        {
            // �ν��Ͻ��� �����ϴ��� üũ
            if (instance == null)
            {
                // �ν��Ͻ��� ������ ã��
                instance = FindObjectOfType<T>();
                // �ν��Ͻ��� ã�Ҵ��� üũ
                if (instance == null)
                {
                    // ���ٸ� ���� ������Ʈ�� ����
                    GameObject obj = new GameObject(typeof(T).Name);
                    // ������ �� ��ü�� TŸ�� ������Ʈ�� ���δ�
                    obj.AddComponent<T>();
                }
            }

            
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // �ν��Ͻ��� ���ٸ�
        if(instance == null)
        {
            // �ν��Ͻ��� �̸� �־��ִ� �۾�
            //  -> Instance ������Ƽ�� ���� ���� �� ��ü�� ã�ų� �����ϴ� ������ ����
            instance = this as T;
            // ���� ����ǵ� ���� ������Ʈ�� �ı����� �ʵ���
            DontDestroyOnLoad(gameObject);
        }
        // �ν��Ͻ��� �ִٸ�
        else
        {
            // �� �������� �ν��Ͻ��� �����ϴ� ���� �߸��� ������� ���� ������ �ν��Ͻ� ����
            Destroy(gameObject);
        }
    }


}
