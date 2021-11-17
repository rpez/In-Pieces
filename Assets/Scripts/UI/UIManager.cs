using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public GameObject m_dialogueWindow;
    public TMP_Text m_dialogueText;
    public GameObject m_dialogueOptionContainer;

    public GameObject m_dialogueOptionPrefab;

    private GameManager m_gameManager;
    private DialogueManager m_dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        m_dialogueManager = (DialogueManager)FindObjectOfType(typeof(DialogueManager));

        StartConversation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartConversation()
    {
        DisplayConversation(m_dialogueManager.StartDialogue("sketch_dialogue", m_gameManager));
        DisplayConversationOptions();
    }

    private void DisplayConversation(IDialogue dialogue)
    {
        m_dialogueText.text = dialogue.ToString();
    }

    private void SelectConversationOption(int x)
    {
        var selected = m_dialogueManager.SelectOption(x, m_gameManager);  // Prints the next conversation node

        // need to check for null here, we might click on something that is out of bounds of this conversation
        if (selected != null)
        {
            DisplayConversation(selected);
            DisplayConversationOptions();    // Prints options for the player to choose
        }
    }

    private void DisplayConversationOptions()
    {
        ClearDialogueOptions();
        int i = 1;

        foreach (IDialogue option in m_dialogueManager.ListOptions(m_gameManager))
        {
            GameObject button = GameObject.Instantiate(m_dialogueOptionPrefab, m_dialogueOptionContainer.transform);
            DialogueOption script = button.GetComponent<DialogueOption>();

            // This is a little error prone atm, will add some failsafe later
            string text = "Placeholder dailogue, very bad if you see this :D";
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

    private void ClearDialogueOptions()
    {
        foreach(Transform child in m_dialogueOptionContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
