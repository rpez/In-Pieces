using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueOption : MonoBehaviour
{
    public TMP_Text m_text;

    private Action m_onClickCallback;

    public void Initialize(string text, Action callback)
    {
        m_text.text = text;
        m_onClickCallback = callback;
    }

    public void OnClick()
    {
        m_onClickCallback.Invoke();
    }
}
