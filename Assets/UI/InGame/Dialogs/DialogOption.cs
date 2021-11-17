using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOption : MonoBehaviour
{
    private string m_text;
    private Action m_onClickCallback;

    public DialogOption(string text, Action onClickCallback)
    {
        m_text = text;
        m_onClickCallback = onClickCallback;
    }
}
