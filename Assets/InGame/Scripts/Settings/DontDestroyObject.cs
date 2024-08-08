using UnityEngine;

public class DontDestroyObject : Singleton<DontDestroyObject>
{
    [SerializeField] private bool _addComponent = true;
    
    protected override void Awake()
    {
        base.Awake();
        InitManager();
    }
    protected void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void InitManager()
    {
        //시작 시 계속 유지되어야 할 매니저들은 여기서 셋팅
        SetSingleton<PoolManager>();
        SetSingleton<UIManager>();
        SetSingleton<DataManager>();
    }

    private void SetSingleton<T>() where T : MonoBehaviour
    {
        if (_addComponent)
        {
            if (gameObject.GetComponent<T>() == null)
            {
                gameObject.AddComponent<T>();
            }
        }
        else
        {
            if (FindObjectOfType<T>() == null)
            {
                var newObject = new GameObject(typeof(T).FullName);
                newObject.transform.SetParent(transform);
                newObject.AddComponent<T>();
            }
        }
    }
    
}