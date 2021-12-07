using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : Singleton<CutsceneManager>
{
    private UIManager m_UI;

    public void PlayCutscene(
        string startText,
        string endText,
        string dialoguename,
        Action midEndTransitionCallback,
        Action transitionEndEndCallback)
    {
        m_UI.StartCutcene(startText, endText, dialoguename, midEndTransitionCallback, transitionEndEndCallback);
    }

    public void InitiateTransition(string transitionText, Action midTransitionCallback, Action transitionEndCallback)
    {
        m_UI.AnimateFadeTransition(transitionText, midTransitionCallback, transitionEndCallback);
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_UI = (UIManager)FindObjectOfType(typeof(UIManager));
    }
}