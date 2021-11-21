using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    private UIManager m_UI;

    public void InitiateTransition(Action midTransitionCallback, Action transitionEndCallback)
    {
        m_UI.AnimateFadeTransition(midTransitionCallback, transitionEndCallback);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_UI = (UIManager)FindObjectOfType(typeof(UIManager));
    }
}