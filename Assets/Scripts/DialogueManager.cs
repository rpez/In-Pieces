using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueFileReader _dfr;
    
    public Dictionary<string, DialogueTree> DialogueTrees { get; private set; }

    void Start()
    {
        _dfr = new DialogueFileReader();
        DialogueTrees = _dfr.ReadAllDialogueFiles();
    }

    /*  Usage example:

        DialogueTree dt = DialogueTrees["sketch_dialogue"];
        List<IDialogue> dialogues = dt.Root.Flatten().ToList();

        foreach (IDialogue d in dialogues)
        {
            if (typeof(IConditionalDialogue).IsAssignableFrom(d.GetType()))
            {
                IConditionalDialogue dialogue = (IConditionalDialogue) d;

                if (ConditionIsTrue(dialogue) != null)
                    Debug.Log(dialogue.Condition + " " + ConditionIsTrue(dialogue));
            }
        }
    */
    public bool? ConditionIsTrue(IConditionalDialogue dialogue)
    {
        if (string.IsNullOrWhiteSpace(dialogue.Condition)) return null;

        GameManager gameManager = (GameManager) FindObjectOfType(typeof(GameManager));

        Match boolMatch = IsBoolCondition(dialogue.Condition);
        Match intMatch = IsIntCondition(dialogue.Condition);

        if (boolMatch.Success)
        {
            string stateVariable = boolMatch.Groups[3].Value;
            bool returnable = gameManager.GetStateValue<bool>(stateVariable);

            return boolMatch.Groups[2].Value == "NOT" ? !returnable : returnable;
        }
        else if (intMatch.Success)
        {
            string stateVariable = intMatch.Groups[3].Value;
            string op = intMatch.Groups[4].Value;
            int checkAgainstVal;
            int.TryParse(intMatch.Groups[5].Value, out checkAgainstVal);

            int val = gameManager.GetStateValue<int>(stateVariable);
            bool returnable;

            if (op.Equals("=="))
                returnable = val == checkAgainstVal;
            else if (op.Equals("<"))
                returnable = val < checkAgainstVal;
            else if (op.Equals(">"))
                returnable = val > checkAgainstVal;
            else if (op.Equals("<="))
                returnable = val <= checkAgainstVal;
            else
                returnable = val >= checkAgainstVal;

            return intMatch.Groups[2].Value == "NOT" ? !returnable : returnable;
        }
        
        Debug.LogError(string.Format("DialogueError: Bad syntax in condition '{0}'", dialogue.Condition));
        return null;
    }

    private Match IsBoolCondition(string condition)
    {
        /*  Match group 1: "IF"
            Match group 2: "NOT" (optional)
            Match group 3: variable
        */
        Regex boolRegex = new Regex(@"^(IF)(?: (NOT))? (\w+)$", RegexOptions.Singleline);

        return boolRegex.Match(condition);
    }

    private Match IsIntCondition(string condition)
    {
        /*  Match group 1: "IF"
            Match group 2: "NOT" (optional)
            Match group 3: variable
            Match group 4: operator
            Match group 5: integer
        */
        Regex intRegex = new Regex(@"^(IF)(?: (NOT))? (\w+) (>=|==|<=|<|>) (\d+)$", RegexOptions.Singleline);

        return intRegex.Match(condition);
    }
}
