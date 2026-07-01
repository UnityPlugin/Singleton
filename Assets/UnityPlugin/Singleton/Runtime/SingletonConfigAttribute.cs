using System;

namespace UnityPlugin
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SingletonConfigAttribute : Attribute
    {
        public string ResourcePath { get; protected set; }
        public bool AutoCreate { get; protected set; }
        public bool DontDestroyOnLoad { get; protected set; }

        public SingletonConfigAttribute(string resourcePath = "", bool autoCreate = true, bool dontDestroyOnLoad = true)
        {
            ResourcePath = resourcePath;
            AutoCreate = autoCreate;
            DontDestroyOnLoad = dontDestroyOnLoad;
        }
    }
}
