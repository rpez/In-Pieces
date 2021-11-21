using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// An example class
public class DialogueUIConsole : MonoBehaviour
{
    private GameManager m_gameManager;
    private DialogueManager m_dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        m_gameManager = (GameManager) FindObjectOfType(typeof(GameManager));
        m_dialogueManager = (DialogueManager) FindObjectOfType(typeof(DialogueManager));

        //StartConversation();
    }

    // Update is called once per frame
    void Update()
    {
        // Numbers from 1 to 9 control the conversation options
        //if (Input.anyKey)
        //{
        //    int x;
        //    bool isNumber = int.TryParse(Input.inputString, out x);

        //    if (isNumber && x > 0 && x < 10)
        //    {
        //        SelectConversationOption(x - 1);
        //    }
        //}
    }

    private void StartConversation()
    {
        Debug.Log(m_dialogueManager.StartDialogue("sketch_dialogue"));  // Prints the first conversation node
        PrintConversationOptions();
    }

    private void SelectConversationOption(int x)
    {
        var selected = m_dialogueManager.SelectOption(x);  // Prints the next conversation node
        
        // need to check for null here, we might click on something that is out of bounds of this conversation
        if (selected != null)
        {
            Debug.Log(selected);
            PrintConversationOptions();    // Prints options for the player to choose
        }
    }

    private void PrintConversationOptions()
    {
        int i = 1;

        foreach (IDialogue option in m_dialogueManager.ListOptions())
            if (option is EndDialogue)
                Debug.Log("<color=white>{END}</color>");       // here we would make the UI draw an End Dialogue button
            else if (option is ContinueDialogue)
                Debug.Log("<color=white>{CONTINUE}</color>");  // here we would make the UI draw a Continue button
            else if (option is PlayerDialogue)
            {
                Debug.Log(string.Format("<color=white>{0}. {1}</color>", i, option));
                i++;
            }
            else
                Debug.Log(option);
    }
}
