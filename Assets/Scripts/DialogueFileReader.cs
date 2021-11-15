using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueFileReader
{
    private bool _hasEndKeyword;  // makes sure there is an escape from conversation
    private DialogueTree _dialogueTree;
    private TreeNode<IDialogue> _currentNode;
    private int _lineNumber;
    private int _indentation;
    private int _prevIndentation;
    private string _filename;

    public DialogueFileReader() {}

    public Dictionary<string, DialogueTree> ReadAllDialogueFiles()
    {
        Dictionary<string, DialogueTree> allDialogueTrees = new Dictionary<string, DialogueTree>();

        // Dialogue files should be located in "Assets/Dialogues/"
        string dialogueFolder = Path.Combine(Application.dataPath, "Dialogues");

        // Regexes that correspond to the different Dialogue classes
        Regex endRegex = new Regex(@"^\{END\}$", RegexOptions.Singleline);
        Regex refRegex = new Regex(@"^\{(LINE) (\d+)(?: (CHILDREN))?\}$", RegexOptions.Singleline);
        Regex actorRegex = new Regex(@"^((?:[\w ()]+)+): ([\w ,.!'""-?]+)(?: \{([\w =><\d+])+\})?(?: \[([\w >=<,+\-\/]+)\])?$", RegexOptions.Singleline);
        Regex playerRegex = new Regex(@"^(?:<(NOSE|EARS|LEGS|HAND|EYES))?> ([\w ,.!'""-?]+)(?: \{([\w =><\d+]+)\})?(?: \[([\w >=<,+\-\/]+)\])?$", RegexOptions.Singleline);

        // Condition and Actions regex
        Regex boolConditionRegex = new Regex(@"^(IF)(?: (NOT))? (\w+)$", RegexOptions.Singleline);
        Regex intConditionRegex = new Regex(@"^(IF)(?: (NOT))? (\w+) (>=|==|<=|<|>) (\d+)$", RegexOptions.Singleline);
        Regex actionsRegex = new Regex(@"^SET \w+ (?:TRUE|FALSE)|ADD \w+ [+-]?\d+|ROLL \d+\/\d+$", RegexOptions.Singleline);

        foreach (string fullPath in Directory.GetFiles(dialogueFolder))
        {
            if (Path.GetExtension(fullPath) != ".dlg") continue;

            _filename = Path.GetFileNameWithoutExtension(fullPath);

            string[] allLines = File.ReadAllLines(fullPath);

            allDialogueTrees.Add(_filename,
                ReadDialogueFile(allLines, endRegex, refRegex, actorRegex, playerRegex, boolConditionRegex, intConditionRegex, actionsRegex));
        }

        return allDialogueTrees;
    }


    /*  Read lines from a dialogue file one by one.
        Creates a single DialogueTree and populates it with the lines read from the dialogue file as Dialogue classes.

        There exists four Dialogue classes:

            1) PlayerDialogue is spoken by the player. It has no Actor, but can have a BodyPart (for dice rolls).
            2) ActorDialogue is spoken by NPCs. They have an Actor.
            3) ReferenceDialogue references dialogue lines. They are used to jump back in conversation.
            4) EndDialogue ends the dialogue.

        WARNING!!! The validation has not been tested and therefore is likely to not be foolproof. DON'T blindly rely on it! */
    private DialogueTree ReadDialogueFile(string[] allLines,
        Regex endRegex, Regex refRegex, Regex actorRegex, Regex playerRegex,
        Regex boolConditionRegex, Regex intConditionRegex, Regex actionsRegex)
    {
        _hasEndKeyword = false;
        _dialogueTree = null;
        _currentNode = null;
        _lineNumber = 1;
        _indentation = 0;

        foreach (string line in allLines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // this line is empty, skip it

            string trimmed = line.Trim();

            if (trimmed.Substring(0, 1) == "#") continue; // this line is a comment, skip it

            // Count indentation. Accept only spaces, no tabs.
            _prevIndentation = _indentation;
            _indentation = line.TakeWhile(c => c.Equals(' ')).Count();

            if ((_indentation - _prevIndentation) % 4 != 0)
            {
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}': Indentation in dialogue files should be 4.", _filename));
                break;
            }

            // OK, ready to read this line with regex.

            IDialogue dialogue;

            Match endMatch = endRegex.Match(trimmed);
            Match refMatch = refRegex.Match(trimmed);
            Match actorMatch = actorRegex.Match(trimmed);
            Match playerMatch = playerRegex.Match(trimmed);
            
            if (endMatch.Success)
                dialogue = HandleEndDialogue(endMatch);
            else if (refMatch.Success)     // this line is a RefDialogue line
                dialogue = HandleRefDialogue(refMatch);
            else if (actorMatch.Success)   // this line is an ActorDialogue line
                dialogue = HandleActorDialogue(actorMatch, boolConditionRegex, intConditionRegex, actionsRegex);
            else if (playerMatch.Success)  // this line is a PlayerDialogue line
                dialogue = HandlePlayerDialogue(playerMatch, boolConditionRegex, intConditionRegex, actionsRegex);
            else                           // error, bad syntax in this line
            {
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}': line " + _lineNumber + " didn't match the dialogue syntax.", _filename));
                continue;
            }

            // Place the dialogue line in the correct place inside DialogueTree.
            AddNodeToDialogueTree(dialogue);

            _lineNumber++;
        }

        if (!_hasEndKeyword)
        {
            Debug.LogError(string.Format("DialogueError: Error when reading '{0}': {END} command missing.", _filename));
            return null;
        }

        return _dialogueTree;
    }

    private void AddNodeToDialogueTree(IDialogue dialogue)
    {
        if (_dialogueTree == null)  // Create a new DialogueTree if one doesn't exist yet.
        {
            _dialogueTree = new DialogueTree(new TreeNode<IDialogue>(id: _lineNumber, dialogue));
            _currentNode = _dialogueTree.Root;
        }
        else if (_indentation > _prevIndentation)  // We went one level lower in the tree. Add child and set it to be the current node.
        {
            _currentNode = _currentNode.AddChild(id: _lineNumber, dialogue);
        }
        else if (_indentation == _prevIndentation)  // We are at the same level in the tree. Add child, don't touch the current node.
        {
            _currentNode.AddChild(id: _lineNumber, dialogue);
        }
        else  // We are n levels higher in the tree. Go up as much as we have to, then add child and set it to be the current node.
        {
            while (_currentNode.Parent != null && _indentation <= _prevIndentation)
            {
                _prevIndentation -= 4;
                _currentNode = _currentNode.Parent;
            }
            _currentNode = _currentNode.AddChild(id: _lineNumber, dialogue);
        }
    }

    private IDialogue HandleActorDialogue(Match match, Regex boolConditionRegex, Regex intConditionRegex, Regex actionsRegex)
    {
        List<string> actions = SplitAndValidateActions(match.Groups[4].Value, actionsRegex);
        IDialogueCondition condition = ValidateCondition(match.Groups[3].Value, boolConditionRegex, intConditionRegex);

        return new ActorDialogue(
            match.Groups[1].Value,  // Actor
            match.Groups[2].Value,  // Line
            condition,              // Condition
            actions                 // Actions
        );
    }

    private IDialogue HandleEndDialogue(Match match)
    {
        _hasEndKeyword = true;
        return new EndDialogue();
    }

    // TO-DO: Some copy-paste here... cleanup
    private IDialogue HandlePlayerDialogue(Match match, Regex boolConditionRegex, Regex intConditionRegex, Regex actionsRegex)
    {
        List<string> actions = SplitAndValidateActions(match.Groups[4].Value, actionsRegex);
        IDialogueCondition condition = ValidateCondition(match.Groups[3].Value, boolConditionRegex, intConditionRegex);

        return new PlayerDialogue(
            match.Groups[1].Value,  // BodyPart
            match.Groups[2].Value,  // Line
            condition,              // Condition
            actions                 // Actions
        );
    }

    private IDialogue HandleRefDialogue(Match match)
    {
        int refLineNumber;
        int.TryParse(match.Groups[2].Value, out refLineNumber);

        if (match.Groups[3].Value == "CHILDREN")
        {
            return new RefDialogue(refLineNumber);
        }

        return new RefDialogue(refLineNumber - 1);
    }

    private List<string> SplitAndValidateActions(String actionsString, Regex actionsRegex)
    {
        if (string.IsNullOrWhiteSpace(actionsString)) return null;

        List<string> actions = null;

        // Split actions into a list
        try
        {
            actions = actionsString.Split(',').Select(s => s.Trim()).ToList();
        }
        catch
        {
            Debug.LogError(string.Format("DialogueError: Error when reading '{0}': line " + _lineNumber + ". Cannot split Actions.", _filename));
        }

        if (actions == null) return actions;

        // Validate each action with regex
        foreach (string a in actions)
        {
            // note: disadvantages of using regex: currently not checking if e.g. in 'ROLL 12/4' the denominator is smaller then the nominator
            Match actionsMatch = actionsRegex.Match(a);

            if (!actionsMatch.Success)
            {
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}': line " + _lineNumber + ". Bad Actions syntax.", _filename));
                actions.Remove(a);
            }
        }

        return actions;
    }

    private IDialogueCondition ValidateCondition(String condition, Regex boolConditionRegex, Regex intConditionRegex)
    {
        if (string.IsNullOrWhiteSpace(condition)) return null;

        Match boolMatch = boolConditionRegex.Match(condition);
        Match intMatch = intConditionRegex.Match(condition);

        bool negator = condition.Contains("NOT");

        if (intMatch.Success)
        {
            string variable = intMatch.Groups[3].Value;
            string op = intMatch.Groups[4].Value;
            int val;
            int.TryParse(intMatch.Groups[5].Value, out val);
            return new IntDialogueCondition(negator, variable, op, val);
        }
        else if (boolMatch.Success)
        {
            string variable = boolMatch.Groups[3].Value;
            return new BoolDialogueCondition(negator, variable);
        }

        Debug.LogError(string.Format("DialogueError: Error when reading '{0}': line " + _lineNumber + ". Bad Condition syntax.", _filename));
        return null;
    }
}
