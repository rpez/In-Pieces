using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    private DialogueFileReader _dfr;
    private string _filename;
    
    public Dictionary<string, DialogueTree> AllDialogue { get; private set; }
    public DialogueTree CurrentDialogue { get; set; }

    void Awake()
    {
        _dfr = new DialogueFileReader();
        AllDialogue = _dfr.ReadAllDialogueFiles();
    }

    /*  Start conversation.
        Return the next conversation line to be drawn on the screen. */
    public IDialogue StartDialogue(string dialogueFile)
    {
        DialogueTree dialogueTree;

        if (AllDialogue.TryGetValue(dialogueFile, out dialogueTree))
        {
            _filename = dialogueFile;
            CurrentDialogue = dialogueTree;
            CurrentDialogue.CurrentLine = GetNextActorDialogue(CurrentDialogue.Root);

            IDialogue dialogue = CurrentDialogue.CurrentLine.Value;
            PerformActions(dialogue);

            return dialogue;
        }

        Debug.LogError(string.Format("DialogueError: Couldn't find dialogue file '{0}'", _filename));
        return null;
    }

    /* Stop conversation */
    public void StopDialogue()
    {
        CurrentDialogue.CurrentLine = null;
        CurrentDialogue = null;
    }

    /* Returns a list of currently available options for the player to interact with. */
    public List<IDialogue> ListOptions()
    {
        if (CurrentDialogue == null) return null;

        CurrentDialogue.Options = AvailableChildren(CurrentDialogue.CurrentLine);

        return CurrentDialogue.Options.Select(x => x.Value).ToList();
    }

    /*  This function is used to select a conversation option in dialogue.
        Return the next conversation line to be drawn on the screen. */
    public IDialogue SelectOption(int x)
    {
        if (CurrentDialogue == null) return null;

        // is selected option out of bounds?
        if (x < 0 || x > CurrentDialogue.Options.Count - 1)
            return null;

        TreeNode<IDialogue> selectedOption = CurrentDialogue.Options[x];

        if (!(selectedOption.Value is ISelectableDialogue))
        {
            int lineNumber = CurrentDialogue.CurrentLine.Id;
            Debug.LogError(string.Format("DialogueError: Error in '{0}'.dlg: line {1} children. Trying to select an unselectable option!", _filename, lineNumber));
            return null;
        }

        PerformActions(selectedOption.Value);

        if (selectedOption.Value is EndDialogue || selectedOption.Children[0].Value is EndDialogue)
        {
            StopDialogue();
            return null;
        }

        CurrentDialogue.CurrentLine = GetNextActorDialogue(selectedOption);
        PerformActions(CurrentDialogue.CurrentLine.Value);

        return CurrentDialogue.CurrentLine.Value;
    }

    private TreeNode<IDialogue> GetNextActorDialogue(TreeNode<IDialogue> parentNode)
    {
        try {
            TreeNode<IDialogue> firstChild = AvailableChildren(parentNode)[0];

            // make sure this is actually an ActorDialogue
            _ = (ActorDialogue) firstChild.Value;

            return firstChild;
        }
        catch (Exception e) when (e is IndexOutOfRangeException || e is InvalidCastException)
        {
            int lineNumber = parentNode.Id;
            Debug.LogError(string.Format("DialogueError: Error in '{0}.dlg': line {1} children. Couldn't find next available ActorDialogue!", _filename, lineNumber));
            return null;
        }
    }

    private List<TreeNode<IDialogue>> AvailableChildren(TreeNode<IDialogue> parentNode, int refFollowCount = 0)
    {
        List<TreeNode<IDialogue>> availableChildren = new List<TreeNode<IDialogue>>();

        foreach (TreeNode<IDialogue> child in parentNode.Children)
        {
            TreeNode<IDialogue> node = child;

            if (node.Value is RefDialogue refDialogue && refDialogue.RefChildren)
            {
                if (refFollowCount >= 5)
                {
                    int lineNumber = node.Id;
                    Debug.LogError(string.Format("DialogueError: Error in '{0}.dlg': line {1}. Nested references limit reached!", _filename, lineNumber));
                    return null;
                }

                List<TreeNode<IDialogue>> refChildren = AvailableChildren(CurrentDialogue.FindById(refDialogue.Ref), refFollowCount + 1);
                availableChildren = availableChildren.Concat(refChildren).ToList();
            }
            else
            {
                if (node.Value is RefDialogue refDialogueLine)
                    node = CurrentDialogue.FindById(refDialogueLine.Ref);

                if (DialogueAvailable(node.Value))
                    availableChildren.Add(node);
            }
        }

        // Check that we don't mix and match different Dialogue types
        bool allActorDialogue = availableChildren.All(x => x.Value is ActorDialogue);
        bool allSelectableDialogue = availableChildren.All(x => x.Value is ISelectableDialogue);

        if (!(allActorDialogue || allSelectableDialogue))
        {
            int lineNumber = parentNode.Id;
            Debug.LogError(string.Format("DialogueError: Error in '{0}.dlg': line {1} children. Can't mix ActorDialogue and SelectableDialogue types!", _filename, lineNumber));
            return null;
        }

        return availableChildren;
    }

    /*  This function is used to check if a dialogue line should be available to the player or not.
        It checks the status of the dialogue line Conditions in GameManager.State */
    private bool DialogueAvailable(IDialogue dialogue)
    {
        if (dialogue is PlayerDialogue playerDialogue && !string.IsNullOrWhiteSpace(playerDialogue.BodyPart))
        {
            bool hasBodyPart = GameManager.Instance.GetStateValue<bool>("HAS_" + playerDialogue.BodyPart);
            if (!hasBodyPart) return false;
        }

        if (dialogue is IConditionalDialogue condDialogue)
            return condDialogue.Conditions.All(x => ConditionIsTrue(x));
        return true;
    }

    /* Check if a single Condition is true */
    private bool ConditionIsTrue(IDialogueCondition condition)
    {
        bool returnable;

        if (condition is BoolDialogueCondition boolCond)
        {
            returnable = GameManager.Instance.GetStateValue<bool>(boolCond.Variable);

            return boolCond.Negator ? !returnable : returnable;
        }

        IntDialogueCondition intCond = (IntDialogueCondition) condition;
        
        int val = GameManager.Instance.GetStateValue<int>(intCond.Variable);

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

    /* Perform all actions. Actions can modify game state. */
    private void PerformActions(IDialogue dialogue)
    {
        if (!(dialogue is IConditionalDialogue))
            return;

        IConditionalDialogue condDialogue = (IConditionalDialogue) dialogue;

        foreach (IDialogueAction action in condDialogue.Actions)
        {
            if (action is SetDialogueAction setAction)
                GameManager.Instance.SetStateValue<bool>(setAction.Variable, setAction.Value);
            else if (action is AddDialogueAction addAction)
            {
                int prevValue = GameManager.Instance.GetStateValue<int>(addAction.Variable);

                // Is the variable we're trying to change a body part attitude? Clamp it between [-3, 3]
                List<string> bodyPartVariables = new List<string>{ "NOSE", "EYES", "EARS", "HAND", "LEGS" };

                if (bodyPartVariables.Contains(addAction.Variable))
                    GameManager.Instance.SetStateValue<int>(addAction.Variable, Mathf.Clamp(prevValue + addAction.Value, -3, 3));
                else
                    GameManager.Instance.SetStateValue<int>(addAction.Variable, prevValue + addAction.Value);
            }
            else if (action is RollDialogueAction rollAction && condDialogue is PlayerDialogue playerDialogue)
            {
                int result = UnityEngine.Random.Range(1, rollAction.Denominator + 1); // roll the dice
                int attitude = GameManager.Instance.GetStateValue<int>(playerDialogue.BodyPart);
                int checkAgainst = rollAction.Denominator - rollAction.Numerator;

                if (result + attitude > checkAgainst)
                {
                    GameManager.Instance.SetStateValue<bool>("ROLL_SUCCESS", true);
                    Debug.Log(string.Format("<color=green>SUCCESS</color><color=white>: Rolled a {0} + {1} vs. {2}</color>", result, attitude, checkAgainst));
                }
                else
                {
                    GameManager.Instance.SetStateValue<bool>("ROLL_SUCCESS", false);
                    Debug.Log(string.Format("<color=red>FAILURE</color><color=white>: Rolled a {0} + {1} vs. {2}</color>", result, attitude, checkAgainst));
                }
            }
        }
    }
}
