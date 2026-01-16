using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool _applicationIsQuitting = false;
    private static readonly object lockObject = new();

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = FindAnyObjectByType<T>();

                        if (instance == null)
                        {
                            GameObject singletonObject = new(typeof(T).Name);
                            instance = singletonObject.AddComponent<T>();
                            DontDestroyOnLoad(singletonObject);
                        }
                    }
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        lock (lockObject)
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = gameObject.GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        _applicationIsQuitting = false;
    }
#endif
}