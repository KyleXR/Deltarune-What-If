using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                //Debug.LogWarning($"{typeof(T)} instance already destroyed on application quit. Returning null.");
                return null;
            }

            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();

                if (instance == null && Application.isPlaying)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    instance = obj.AddComponent<T>();
                    //Debug.Log($"{typeof(T).Name} instance created.");
                }
                else
                {
                    //Debug.Log($"{typeof(T).Name} instance found: {instance.gameObject.name}");
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
            DontDestroyOnLoad(gameObject);
            //Debug.Log($"{typeof(T).Name} instance set and marked as DontDestroyOnLoad.");
        }
        else if (instance != this)
        {
            //Debug.Log($"{typeof(T).Name} instance already exists, destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }
}
