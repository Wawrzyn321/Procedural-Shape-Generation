using UnityEngine;

namespace PSG
{
    public static class ExtensionMethods
    {
        //returns component of given type, adding it in case it's not already attached
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}