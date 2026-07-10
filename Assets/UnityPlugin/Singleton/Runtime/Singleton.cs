using System.Reflection;
using UnityEngine;

namespace UnityPlugin
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T _instance;

        static bool _configUpdated;
        static string _resPath;
        static bool _autoCreate = true;
        static bool _dontDestroyOnLoad = true;

        static bool _destroyed;

        public static T Instance => GetInstance();

        public static T CreateInstance(bool forceCreate = false)
        {
            if (_instance == null)
            {
                UpdateConfig();

                if (!forceCreate && !_autoCreate) return null;

                if (!string.IsNullOrEmpty(_resPath))
                {
                    var prefab = Resources.Load<T>(_resPath);
                    if (prefab)
                    {
                        _instance = Instantiate(prefab);
                        if (_instance) _instance.name = prefab.name;
                    }
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
                if (Application.isPlaying) Destroy(_instance.gameObject);
                else DestroyImmediate(_instance.gameObject);
                _instance = null;
                _destroyed = true;
            }
        }

        protected virtual void Awake()
        {
            if (_instance && _instance != this)
            {
                if (Application.isPlaying) Destroy(_instance.gameObject);
                else DestroyImmediate(_instance.gameObject);
                return;
            }

            _instance = this as T;

            UpdateConfig();
            if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _destroyed = true;
            }
        }


        static T GetInstance()
        {
            if (_instance) return _instance;

#if UNITY_2020_1_OR_NEWER
            _instance = FindAnyObjectByType<T>();
#else
            _instance = FindObjectOfType<T>();
#endif

            if (_instance) return _instance;
            if (_destroyed) return null;

            UpdateConfig();

            if (_autoCreate) return CreateInstance(false);

            return null;
        }

        static void UpdateConfig()
        {
            if (!_configUpdated)
            {
                var configAttribute = typeof(T).GetCustomAttribute<SingletonConfigAttribute>();
                if (configAttribute != null)
                {
                    _resPath = configAttribute.ResourcePath;
                    _autoCreate = configAttribute.AutoCreate;
                    _dontDestroyOnLoad = configAttribute.DontDestroyOnLoad;
                }

                _configUpdated = true;
            }
        }
    }
}
