using System;
using System.Dynamic;
using UnityEngine;

namespace UnityPlugin
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static bool _destroyed;
        static T _instance;

        public static T Instance
        {
            get => _instance ?? CreateInstance();
        }

        public static T CreateInstance(string resPath = null)
        {
            if (_instance == null && !_destroyed)
            {
                _instance = FindAnyObjectByType<T>();
                if (_instance == null && !string.IsNullOrEmpty(resPath))
                {
                    _instance = Resources.Load<T>(resPath);
                }

                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
            }

            return _instance;
        }

        public static void DestroyInstance()
        {
            if (_instance)
            {
                Destroy(_instance.gameObject);
                _instance = null;
                _destroyed = true;
            }
        }

        protected virtual void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _destroyed = true;
            }
        }
    }
}
