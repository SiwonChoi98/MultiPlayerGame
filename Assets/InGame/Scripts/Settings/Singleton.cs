using Mirror;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private bool _isDontDestroy;
    protected bool IsDonDestroy
    {
        get => _isDontDestroy;
        set => _isDontDestroy = value;
    }
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // Try to find existing instance
                instance = FindObjectOfType<T>();

                // If no instance found, create a new one
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            if (_isDontDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}