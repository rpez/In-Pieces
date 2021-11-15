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

    // Modify GameState directly like this
    gameManager.SetStateValue("TUTORIAL_COUNT_SENSES", 3);

    // Check the values of each Condition like this
    foreach (IDialogue d in dialogues)
    {
        if (d is IConditionalDialogue dialogue)
        {
            bool? cond = DialogueAvailable(dialogue, gameManager);
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
    }

    /*  This function is used to check if a dialogue line should be available to the player or not.

        It checks the status of the dialogue line Condition from GameManager.State

        Returns a nullable boolean.
            => This function *should* return null only if
                1) there was no DialogueCondition provided or
                2) there was no equivalent property to DialogueCondition in GameManager.State
    */
    public bool? DialogueAvailable(IConditionalDialogue dialogue, GameManager gameManager)
    {
        if (dialogue.Condition == null) return null;

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

        Debug.LogError(string.Format("DialogueError: Bad syntax in condition '{0}'", dialogue.Condition));
        return null;
    }
}
