using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueOption : MonoBehaviour
{
    public TMP_Text m_text;
    public Image m_bodyPartImage;

    private Action m_onClickCallback;


    public void Initialize(string text, Action callback, Sprite actorImage = null)
    {
        m_text.text = text;
        m_onClickCallback = callback;
        if (actorImage != null)
        {
            m_bodyPartImage.enabled = true;
            m_bodyPartImage.sprite = actorImage;
        }
        else
        {
            m_bodyPartImage.enabled = false;
        }
        
    }

    public void OnClick()
    {
        m_onClickCallback.Invoke();
    }
}
