using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

/*  DialogueManager usage example:

    // Get GameManager
    GameManager gameManager = (GameManager) FindObjectOfType(typeof(GameManager));

    // Assume AllDialogue exists, get the DialogueTree for file "sketch_dialogue.dlg"
    DialogueTree dt = AllDialogue["sketch_dialogue"];

    // Get a list of all dialogues in this DialogueTree
    List<IDialogue> dialogues = dt.ToList();

    // Check the values of each Condition like this
    foreach (IDialogue d in dialogues)
    {
        if (d is IConditionalDialogue dialogue)
        {
            bool cond = DialogueAvailable(dialogue, gameManager);
            if (cond != null)
                Debug.Log(dialogue.Condition + " " + cond);
            PerformActions(dialogue, gameManager);  // Perform actions inside the Dialogue
        }
    }

    // Check the values of each Condition like this
    foreach (IDialogue d in dialogues)
    {
        if (d is IConditionalDialogue dialogue)
        {
            bool cond = DialogueAvailable(dialogue, gameManager);
            if (cond != null)
                Debug.Log(dialogue.Condition + " " + cond);
        }
    }
*/
public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueFileReader _dfr;
    
    public Dictionary<string, DialogueTree> AllDialogue { get; private set; }

    void Start()
    {
        _dfr = new DialogueFileReader();
        AllDialogue = _dfr.ReadAllDialogueFiles();

        // Get GameManager
        GameManager gameManager = (GameManager) FindObjectOfType(typeof(GameManager));

        // Assume AllDialogue exists, get the DialogueTree for file "sketch_dialogue.dlg"
        DialogueTree dt = AllDialogue["sketch_dialogue"];

        // Get a list of all dialogues in this DialogueTree
        List<IDialogue> dialogues = dt.ToList();

        // Check the values of each Condition like this
        foreach (IDialogue d in dialogues)
        {
            if (d is IConditionalDialogue dialogue)
            {
                bool? cond = DialogueAvailable(dialogue, gameManager);
                if (cond != null)
                    Debug.Log(dialogue.Condition + " " + cond);
                PerformActions(dialogue, gameManager);  // Perform actions inside the Dialogue
            }
        }
    }

    /*  This function is used to check if a dialogue line should be available to the player or not.
        It checks the status of the dialogue line Condition in GameManager.State */
    public bool DialogueAvailable(IConditionalDialogue dialogue, GameManager gameManager)
    {
        if (dialogue.Condition == null) return true;

        if (dialogue.Condition is BoolDialogueCondition boolCond)
        {
            bool returnable = gameManager.GetStateValue<bool>(boolCond.Variable);

            return boolCond.Negator ? !returnable : returnable;
        }
        else if (dialogue.Condition is IntDialogueCondition intCond)
        {
            int val = gameManager.GetStateValue<int>(intCond.Variable);
            bool returnable;

            if (intCond.Operator.Equals("=="))
                returnable = val == intCond.Value;
            else if (intCond.Operator.Equals("<"))
                returnable = val < intCond.Value;
            else if (intCond.Operator.Equals(">"))
                returnable = val > intCond.Value;
            else if (intCond.Operator.Equals("<="))
                returnable = val <= intCond.Value;
            else
                returnable = val >= intCond.Value;

            return intCond.Negator ? !returnable : returnable;
        }
    }

    public void PerformActions(IConditionalDialogue dialogue, GameManager gameManager)
    {
        foreach (IDialogueAction action in dialogue.Actions)
        {
            if (action is SetDialogueAction setAction)
                gameManager.SetStateValue<bool>(setAction.Variable, setAction.Value);
            else if (action is AddDialogueAction addAction)
            {
                int prevValue = gameManager.GetStateValue<int>(addAction.Variable);

                // Is the variable we're trying to change a body part attitude? Clamp it between [-3, 3]
                List<string> bodyPartVariables = new List<string>{ "NOSE", "EYES", "EARS", "HAND", "LEGS" };

                if (bodyPartVariables.Contains(addAction.Variable))
                    gameManager.SetStateValue<int>(addAction.Variable, Mathf.Clamp(prevValue + addAction.Value, -3, 3));
                else
                    gameManager.SetStateValue<int>(addAction.Variable, prevValue + addAction.Value);
            }
            else if (action is RollDialogueAction rollAction && dialogue is PlayerDialogue playerDialogue)
            {
                int attitude = gameManager.GetStateValue<int>(playerDialogue.BodyPart);
                int result = UnityEngine.Random.Range(1, rollAction.Denominator + 1); // roll the dice

                if (result + attitude > rollAction.Denominator - rollAction.Numerator)
                {
                    gameManager.SetStateValue<bool>("ROLL_SUCCESS", true);
                }
                else
                    gameManager.SetStateValue<bool>("ROLL_SUCCESS", false);
            }
        }
    }
}
