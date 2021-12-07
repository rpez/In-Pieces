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
    public TMP_Text m_descriptionText;
    public GameObject m_dialogueOptionContainer;
    public PlayableDirector m_director;
    public TMP_Text m_actor;
    public GameObject m_dialogView;
    public GameObject m_descriptionView;

    // Prefab for the dialogue options, set this in editor
    public GameObject m_dialogueOptionPrefab;

    public Sprite[] m_bodypartImages;
    private Dictionary<string, int> m_bodypartNameToImageIndex = new Dictionary<string, int>();

    // For storing callbacks
    private Action m_onDialogueEnd;
    private Action m_onTransitionMid;
    private Action m_onTransitionEnd;

    private string m_rollResult;

    // Start is called before the first frame update
    void Start()
    {
        m_director = GetComponent<PlayableDirector>();

        m_bodypartNameToImageIndex.Add("EYES", 0);
        m_bodypartNameToImageIndex.Add("EARS", 1);
        m_bodypartNameToImageIndex.Add("LEGS", 2);
        m_bodypartNameToImageIndex.Add("HAND", 3);
        m_bodypartNameToImageIndex.Add("NOSE", 4);
        m_bodypartNameToImageIndex.Add("", 5);
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
        TMP_Text dialogText;

        // Check if the dialogue has an actor, if yes, update it to UI
        if (dialogue is ActorDialogue actorDialogue && !actorDialogue.Actor.Equals("DESCRIPTION"))
        {
            m_dialogView.SetActive(true);
            m_descriptionView.SetActive(false);
            dialogText = m_dialogueText;
            m_actor.text = actorDialogue.Actor;
        }
        else
        {
            m_dialogView.SetActive(false);
            m_descriptionView.SetActive(true);
            dialogText = m_descriptionText;
        }

        if (dialogue is IConditionalDialogue condDialogue)
        {
            dialogText.text = m_rollResult + condDialogue.Line;
            m_rollResult = "";
        }
    }

    // Selects a conversation option with index x
    // Then updates the window
    private void SelectConversationOption(int x, string extra = "")
    {
        var selected = DialogueManager.Instance.SelectOption(x);  // Prints the next conversation node

        // need to check for null here, we might click on something that is out of bounds of this conversation
        if (selected != null)
        {
            m_rollResult = extra;
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
            GameObject button = GameObject.Instantiate(
                m_dialogueOptionPrefab,
                m_dialogueOptionContainer.transform);
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
            else if (option is PlayerDialogue playerOption)
            {
                text = string.Format("<color=white>{0}</color>", playerOption.Line);
                int index = i - 1;
                string extra = "";

                if (option is IConditionalDialogue cOption)
                {
                    foreach (IDialogueAction action in cOption.Actions)
                    {
                        if (action is RollDialogueAction rollAction && cOption is PlayerDialogue playerDialogue)
                        {
                            int result = UnityEngine.Random.Range(1, rollAction.Denominator + 1); // roll the dice
                            int attitude = GameManager.Instance.GetStateValue<int>(playerDialogue.BodyPart);
                            int checkAgainst = rollAction.Denominator - rollAction.Numerator;

                            if (result + attitude > checkAgainst)
                            {
                                GameManager.Instance.SetStateValue<bool>("ROLL_SUCCESS", true);
                                extra = string.Format("<color=green>SUCCESS</color><color=white>: ");
                            }
                            else
                            {
                                GameManager.Instance.SetStateValue<bool>("ROLL_SUCCESS", false);
                                extra = string.Format("<color=red>FAILURE</color><color=white>: ");
                            }
                        }
                    }
                }

                callback = () => SelectConversationOption(index, extra);
                i++;
            }
            else
            {
                text = option.ToString();
                callback = () => SelectConversationOption(0);
            }

            if (option is PlayerDialogue)
            {
                script.Initialize(text, callback, m_bodypartImages[m_bodypartNameToImageIndex[(option as PlayerDialogue).BodyPart]]);
            }
            else
            {
                script.Initialize(text, callback);
            }
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
