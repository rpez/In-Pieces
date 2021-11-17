using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueFileReader _dfr;
    
    public Dictionary<string, DialogueTree> AllDialogue { get; private set; }
    public DialogueTree CurrentDialogue { get; set; }

    void Start()
    {
        _dfr = new DialogueFileReader();
        AllDialogue = _dfr.ReadAllDialogueFiles();
    }

    /*  Start conversation.
        Return the next conversation line to be drawn on the screen. */
    public IDialogue StartDialogue(string dialogueFile, GameManager gameManager)
    {
        DialogueTree dialogueTree;

        if (AllDialogue.TryGetValue(dialogueFile, out dialogueTree))
        {
            CurrentDialogue = dialogueTree;
            IDialogue dialogue = CurrentDialogue.CurrentLine.Value;

            PerformActions(dialogue, gameManager);
            return dialogue;
        }

        Debug.LogError(string.Format("ConversationError: Couldn't find dialogue file '{0}'", dialogueFile));
        return null;
    }

    public void StopDialogue()
    {
        CurrentDialogue.CurrentLine = CurrentDialogue.Root;
        CurrentDialogue = null;
    }

    /* Returns a list of currently available options for the player to interact with. */
    public List<IDialogue> ListOptions(GameManager gameManager)
    {
        if (CurrentDialogue == null) return null;

        TreeNode<IDialogue> firstChild = CurrentDialogue.CurrentLine.Children[0];

        if (firstChild.Value is RefDialogue refDialogue)
            CurrentDialogue.Options = AvailableChildren(CurrentDialogue.FindById(refDialogue.Ref), gameManager);
        else
            CurrentDialogue.Options = AvailableChildren(CurrentDialogue.CurrentLine, gameManager);

        return CurrentDialogue.Options.Select(x => x.Value).ToList();
    }

    /*  This function is used to select a conversation option in dialogue.
        Return the next conversation line to be drawn on the screen. */
    public IDialogue SelectOption(int x, GameManager gameManager)
    {
        if (CurrentDialogue == null) return null;

        // is selected option out of bounds?
        if (x < 0 || x > CurrentDialogue.Options.Count - 1)
            return null;

        TreeNode<IDialogue> selectedOption = CurrentDialogue.Options[x];

        // end conversation
        if (selectedOption.Value is EndDialogue)
        {
            StopDialogue();
            return null;
        }
        else if (selectedOption.Value is PlayerDialogue ||
                 selectedOption.Value is ContinueDialogue)
        {
            PerformActions(selectedOption.Value, gameManager);

            TreeNode<IDialogue> nextLine = null;

            foreach (TreeNode<IDialogue> node in selectedOption.Children)
            {
                IDialogue dialogue = node.Value;

                if (DialogueAvailable(dialogue, gameManager))
                {
                    nextLine = node;
                    break;
                }
            }

            if (nextLine.Value is EndDialogue endDialogue)
            {
                StopDialogue();
                return null;
            }
            else if (nextLine.Value is RefDialogue refDialogue)
                nextLine = CurrentDialogue.FindById(refDialogue.Ref);
            else if (nextLine == null)
            {
                Debug.LogError("DialogueError: Couldn't find the next dialogue line!");
                return null;
            }

            CurrentDialogue.CurrentLine = nextLine;
            PerformActions(CurrentDialogue.CurrentLine.Value, gameManager);
        }
        else
            Debug.LogError("DialogueError: Shouldn't be able to select an ActorDialogue!");

        return CurrentDialogue.CurrentLine.Value;
    }

    private List<TreeNode<IDialogue>> AvailableChildren(TreeNode<IDialogue> parentNode, GameManager gameManager)
    {
        List<TreeNode<IDialogue>> availableChildren = new List<TreeNode<IDialogue>>();

        foreach (TreeNode<IDialogue> node in parentNode.Children)
        {
            TreeNode<IDialogue> potentialNode = node;

            if (node.Value is RefDialogue refDialogue)
                potentialNode = CurrentDialogue.FindById(refDialogue.Ref);

            if (DialogueAvailable(potentialNode.Value, gameManager))
                availableChildren.Add(potentialNode);
        }

        return availableChildren;
    }

    /*  This function is used to check if a dialogue line should be available to the player or not.
        It checks the status of the dialogue line Condition in GameManager.State */
    private bool DialogueAvailable(IDialogue dialogue, GameManager gameManager)
    {
        if (!(dialogue is IConditionalDialogue))
            return true;

        IConditionalDialogue condDialogue = (IConditionalDialogue) dialogue;

        if (condDialogue.Condition is BoolDialogueCondition boolCond)
        {
            bool returnable = gameManager.GetStateValue<bool>(boolCond.Variable);

            return boolCond.Negator ? !returnable : returnable;
        }
        else if (condDialogue.Condition is IntDialogueCondition intCond)
        {
            int val = gameManager.GetStateValue<int>(intCond.Variable);
            bool returnable;

            // if there's a better way to do this, would be cool to know!
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

        return true;
    }

    private void PerformActions(IDialogue dialogue, GameManager gameManager)
    {
        if (!(dialogue is IConditionalDialogue))
            return;

        IConditionalDialogue condDialogue = (IConditionalDialogue) dialogue;

        foreach (IDialogueAction action in condDialogue.Actions)
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
            else if (action is RollDialogueAction rollAction && condDialogue is PlayerDialogue playerDialogue)
            {
                int result = UnityEngine.Random.Range(1, rollAction.Denominator + 1); // roll the dice
                int attitude = gameManager.GetStateValue<int>(playerDialogue.BodyPart);
                int checkAgainst = rollAction.Denominator - rollAction.Numerator;

                if (result + attitude > checkAgainst)
                {
                    gameManager.SetStateValue<bool>("ROLL_SUCCESS", true);
                    Debug.Log(string.Format("<color=green>SUCCESS</color><color=white>: Rolled a {0} + {1} vs. {2}</color>", result, attitude, checkAgainst));
                }
                else
                {
                    gameManager.SetStateValue<bool>("ROLL_SUCCESS", false);
                    Debug.Log(string.Format("<color=red>FAILURE</color><color=white>: Rolled a {0} + {1} vs. {2}</color>", result, attitude, checkAgainst));
                }
            }
        }
    }
}
