using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : Singleton<CutsceneManager>
{
    private UIManager m_UI;

    public void PlayCutscene()
    {

    }

    public void InitiateTransition(Action midTransitionCallback, Action transitionEndCallback)
    {
        m_UI.AnimateFadeTransition(midTransitionCallback, transitionEndCallback);
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_UI = (UIManager)FindObjectOfType(typeof(UIManager));
    }
}