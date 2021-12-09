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
        float length,
        float fadeOutLength,
        float fadeInLength,
        string dialoguename,
        Action midEndTransitionCallback,
        Action transitionEndEndCallback)
    {
        m_UI.StartCutcene(startText, 
            endText,
            length,
            fadeOutLength,
            fadeInLength,
            dialoguename, 
            midEndTransitionCallback, 
            transitionEndEndCallback);
    }

    public void StartForcedDialogue(string dialogueName, Action onEnd)
    {
        m_UI.StartConversation(dialogueName, onEnd);
    }

    public void InitiateTransition(
        string transitionText,
        float length,
        float fadeOutLength,
        float fadeInLength,
        Action midTransitionCallback,
        Action transitionEndCallback)
    {
        m_UI.AnimateFadeTransition(transitionText,
            length,
            fadeOutLength,
            fadeInLength,
            midTransitionCallback, 
            transitionEndCallback);
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_UI = (UIManager)FindObjectOfType(typeof(UIManager));
    }
}