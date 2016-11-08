using UnityEngine;

public static class ExtensionMethods{

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
