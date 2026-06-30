using UnityEngine;

namespace UnityPlugin
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        protected static T _instance;

        static bool _destroyed;

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
                    var prefab = Resources.Load<T>(resPath);
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
            DontDestroyOnLoad(gameObject);

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
    }
}
