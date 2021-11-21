using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Playables;

public class UIManager : MonoBehaviour
{
    // These references are set in editor, they are children of the canvas
    public GameObject m_dialogueWindow;
    public TMP_Text m_dialogueText;
    public GameObject m_dialogueOptionContainer;
    public PlayableDirector m_director;

    // Prefab for the dialogue options, set this in editor
    public GameObject m_dialogueOptionPrefab;

    // For storing callbacks
    private Action m_onDialogueEnd;
    private Action m_onTransitionMid;
    private Action m_onTransitionEnd;

    // Start is called before the first frame update
    void Start()
    {
        m_director = GetComponent<PlayableDirector>();
    }

    // Starts (or continues) dialogue
    // dialogueName indicates the .dlg file name
    // onEndCallback is called when the dialogue ends
    public void StartConversation(string dialogueName, Action onEndCallback)
    {
        m_onDialogueEnd = onEndCallback;
        m_dialogueWindow.SetActive(true);
        DisplayConversation(DialogueManager.Instance.StartDialogue(dialogueName));
        DisplayConversationOptions();
    }

    // Updates the dialogue text
    private void DisplayConversation(IDialogue dialogue)
    {
        m_dialogueText.text = dialogue.ToString();
    }

    // Selects a conversation option with index x
    // Then updates the window
    private void SelectConversationOption(int x)
    {
        var selected = DialogueManager.Instance.SelectOption(x);  // Prints the next conversation node

        // need to check for null here, we might click on something that is out of bounds of this conversation
        if (selected != null)
        {
            DisplayConversation(selected);
            DisplayConversationOptions();    // Prints options for the player to choose
        }
        else
        {
            EndConversation();
        }  
    }

    // Shows the possible dialogue options for the player
    private void DisplayConversationOptions()
    {
        ClearDialogueOptions();
        int i = 1;

        foreach (IDialogue option in DialogueManager.Instance.ListOptions())
        {
            GameObject button = GameObject.Instantiate(m_dialogueOptionPrefab, m_dialogueOptionContainer.transform);
            DialogueOption script = button.GetComponent<DialogueOption>();

            // This is a little error prone atm, will add some failsafe later
            string text = "Placeholder dialogue, very bad if you see this :D";
            Action callback = () => { };

            if (option is EndDialogue)
            {
                text = "<color=white>{END}</color>";
                callback = () => SelectConversationOption(0);
            }
            else if (option is ContinueDialogue)
            {
                text = "<color=white>{CONTINUE}</color>";
                callback = () => SelectConversationOption(0);
            }
            else if (option is PlayerDialogue)
            {
                text = string.Format("<color=white>{0}. {1}</color>", i, option);
                int index = i - 1;
                callback = () => SelectConversationOption(index);
                i++;
            }
            else
            {
                text = option.ToString();
                callback = () => SelectConversationOption(0);
            }

            script.Initialize(text, callback);
        }
    }

    // Clears the dialogue options from the scroll list container
    // If this isn't done, old options would stay in the list
    private void ClearDialogueOptions()
    {
        foreach(Transform child in m_dialogueOptionContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Called when conversation ends
    // Calls the provided callback
    private void EndConversation()
    {
        m_onDialogueEnd.Invoke();
        m_dialogueWindow.SetActive(false);
    }

    public void AnimateFadeTransition(Action onMid, Action onEnd)
    {
        m_onTransitionMid = onMid;
        m_onTransitionEnd = onEnd;

        m_director.time = 0;
        m_director.Play();
    }

    public void TransitionMidwayTrigger()
    {
        if (m_onTransitionMid != null) m_onTransitionMid.Invoke();
        m_onTransitionMid = null;
    }

    public void TransitionEndTrigger()
    {
        if (m_onTransitionEnd != null) m_onTransitionEnd.Invoke();
        m_onTransitionEnd = null;
    }
}
