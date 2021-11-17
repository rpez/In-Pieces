using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        State = new GameState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public T GetStateValue<T>(string propertyName)
    {
        return State.GetPropertyValue<T>(propertyName);
    }

    public void SetStateValue<T>(string propertyName, T val)
    {
        State.SetPropertyValue<T>(propertyName, val);
    }
}

public static class ExtensionMethods
{
    public static T GetPropertyValue<T>(this object obj, string propertyName)
    {
        return (T) obj.GetType().GetProperty(propertyName).GetValue(obj, null);
    }

    public static void SetPropertyValue<T>(this object obj, string propertyName, T val)
    {
        PropertyInfo property = obj.GetType().GetProperty(propertyName);

        if (property == null)
            Debug.LogError(string.Format("SetPropertyValueError: Couldn't find property with name '{0}'", propertyName));

        property.SetValue(obj, val);
    }
}
