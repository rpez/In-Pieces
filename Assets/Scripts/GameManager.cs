using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState State { get; set; }

    // Start is called before the first frame update
    void Awake()
    {
        State = new GameState();
    }

    private void Start()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.SetMovementActive(false);

        bool DEBUG_SKIP_INTRO_CUTSCENE = false;

        if (DEBUG_SKIP_INTRO_CUTSCENE)
        {
            GameManager.Instance.State.HAS_EYES = true;
            GameManager.Instance.State.HAS_NOSE = true;
            player.UpdateBodyParts();
            player.SetMovementActive(true);
            return;
        }

        CutsceneManager.Instance.PlayCutscene("Somewhere in a bar far, far away... (in HALIFAX, CANADA)",
            "",
            "intro_dialogue",
            () => {
                
            },
            () => {
                CutsceneManager.Instance.StartForcedDialogue("sketch_dialogue", () => {
                    player.SetMovementActive(true);
                });
            }
        );
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

    public bool ToggleStateValue(string propertyName)
    {
        SetStateValue<bool>(propertyName, !GetStateValue<bool>(propertyName));
        return GetStateValue<bool>(propertyName);
    }
}

public static class ExtensionMethods
{
    public static T GetPropertyValue<T>(this object obj, string propertyName)
    {
        try {
            return (T) obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
        catch (NullReferenceException)
        {
            Debug.LogError(string.Format("GetPropertyValueError: Couldn't find variable '{0}' with type '{1}' in GameManager.State", propertyName, typeof(T)));
            return default(T);
        }
    }

    public static void SetPropertyValue<T>(this object obj, string propertyName, T val)
    {
        try {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);
            property.SetValue(obj, val);
        }
        catch (NullReferenceException)
        {
            Debug.LogError(string.Format("SetPropertyValueError: Couldn't find variable '{0}' with type '{1}' in GameManager.State", propertyName, typeof(T)));
        }            
    }
}
