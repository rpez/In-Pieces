using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPanel : MonoBehaviour
{
    public TMP_Text m_text;
    public float m_relativeTextFadeTime;

    private Image m_image;

    private Action m_onTransitionMid;
    private Action m_onTransitionEnd;

    private float m_currentTime;
    private bool m_transtionOnGoing;
    private bool m_fadeOut;
    private bool m_hold;
    private bool m_fadeIn;

    private bool m_textFadeIn;
    private float m_textTime;

    private float m_length;
    private float m_fadeOutLength;
    private float m_fadeInLength;

    /* Start fade in/out transtion
     * length = how long the screen stays completely black
     * fadeOutSpeed = how fast the screen will go black
     * fadeInSpeed = how fast does the screen fade in from black
     */
    public void StartTransition(
        string text,
        float length,
        float fadeOutLength,
        float fadeInLength,
        Action onTransitionMid,
        Action onTransitionEnd)
    {
        m_text.text = text;

        m_length = length;
        m_fadeOutLength = fadeOutLength;
        m_fadeInLength = fadeInLength;

        m_onTransitionMid = onTransitionMid;
        m_onTransitionEnd = onTransitionEnd;

        m_textTime = length * m_relativeTextFadeTime;

        m_textFadeIn = true;
        m_transtionOnGoing = true;
        m_fadeOut = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_transtionOnGoing) return;

        m_currentTime += Time.deltaTime;

        if (m_fadeOut)
        {
            if (m_currentTime <= m_fadeOutLength)
            {
                float t = m_currentTime / m_fadeOutLength;
                float alpha = Mathf.Lerp(0f, 1f, t);
                ChangeImageAlpha(alpha);
            }
            else
            {
                ChangeImageAlpha(1f);
                m_fadeOut = false;
                m_hold = true;
                m_currentTime = 0f;
                m_onTransitionMid.Invoke();
            }
        }
        else if (m_hold)
        {
            if (m_currentTime > m_length)
            {
                ChangeTextAlpha(0f);
                m_hold = false;
                m_fadeIn = true;
                m_currentTime = 0f;
            }
            else if (m_textFadeIn)
            {
                if (m_currentTime < m_textTime)
                {
                    float t = m_currentTime / m_textTime;
                    float alpha = Mathf.Lerp(0f, 1f, t);
                    ChangeTextAlpha(alpha);
                }
                else
                {
                    ChangeTextAlpha(1f);
                    m_textFadeIn = false;
                }
            }
            else
            {
                if (m_currentTime > m_length - m_textTime)
                {
                    float t = m_currentTime / m_length;
                    float alpha = Mathf.Lerp(1f, 0f, t);
                    ChangeTextAlpha(alpha);
                }
            }
        }
        if (m_fadeIn)
        {
            if (m_currentTime <= m_fadeInLength)
            {
                float t = m_currentTime / m_fadeInLength;
                float alpha = Mathf.Lerp(1f, 0f, t);
                ChangeImageAlpha(alpha);
            }
            else
            {
                ChangeImageAlpha(0f);
                m_fadeIn = false;
                m_transtionOnGoing = false;
                m_currentTime = 0f;
                m_onTransitionEnd.Invoke();
                gameObject.SetActive(false);
            }
        }
    }

    private void ChangeImageAlpha(float value)
    {
        m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, value);
    }

    private void ChangeTextAlpha(float value)
    {
        m_text.color = new Color(m_text.color.r, m_text.color.g, m_text.color.b, value);
    }
}
