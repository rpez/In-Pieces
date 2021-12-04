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

    // used for validation
    private Dictionary<string, Regex> _regexes = new Dictionary<string, Regex>
    {
        { "endRegex", new Regex(@"^\{END\}$", RegexOptions.Singleline) },
        { "continueRegex", new Regex(@"^\{CONTINUE\}$", RegexOptions.Singleline) },
        { "refRegex", new Regex(@"^\{(LINE) (\d+)(?: (CHILDREN))?\}$", RegexOptions.Singleline) },
        { "actorRegex", new Regex(@"^((?:[\w ()]+)+): ([\w ,.!'""-?]+)(?: \{(IF [\w =><\d+]+)\})?(?: \[([\w >=<,+\-\/]+)\])?$", RegexOptions.Singleline) },
        { "playerRegex", new Regex(@"^(?:<(NOSE|EARS|LEGS|HAND|EYES))?> ([\w ,.!'""-?]+)(?: \{(IF [\w =><\d+]+)\})?(?: \[([\w >=<,+\-\/]+)\])?$", RegexOptions.Singleline) },
        { "boolConditionRegex", new Regex(@"^(IF)(?: (NOT))? (\w+)$", RegexOptions.Singleline) },
        { "intConditionRegex", new Regex(@"^(IF)(?: (NOT))? (\w+) (>=|==|<=|<|>) (\d+)$", RegexOptions.Singleline) },
        { "setActionRegex", new Regex(@"^(SET) (\w+) ((?:TRUE|FALSE))$", RegexOptions.Singleline) },
        { "addActionRegex", new Regex(@"^(ADD) (\w+) ([+-]?\d+)$", RegexOptions.Singleline) },
        { "toggleActionRegex", new Regex(@"^(TGL) (\w+)$", RegexOptions.Singleline) },
        { "rollActionRegex", new Regex(@"^(ROLL) (\d+)\/(\d+)$", RegexOptions.Singleline) },
        { "bodyPartRegex", new Regex(@"^NOSE|EARS|LEGS|HAND|EYES$", RegexOptions.Singleline) },
    };

    public DialogueFileReader() {}

    public Dictionary<string, DialogueTree> ReadAllDialogueFiles()
    {
        Dictionary<string, DialogueTree> allDialogue = new Dictionary<string, DialogueTree>();

        // Dialogue files should be located in "Assets/Dialogues/"
        string dialogueFolder = Path.Combine(Application.streamingAssetsPath, "Dialogues");

        foreach (string fullPath in Directory.GetFiles(dialogueFolder))
        {
            if (Path.GetExtension(fullPath) != ".dlg") continue;

            _filename = Path.GetFileNameWithoutExtension(fullPath);

            string[] allLines = File.ReadAllLines(fullPath);

            allDialogue.Add(_filename,
                ReadDialogueFile(allLines));
        }

        return allDialogue;
    }


    /*  Read lines from a dialogue file one by one.
        Creates a single DialogueTree and populates it with the lines read from the dialogue file as Dialogue classes.

        There exists four Dialogue classes:

            1) PlayerDialogue is spoken by the player. It has no Actor, but can have a BodyPart (for dice rolls).
            2) ActorDialogue is spoken by NPCs. They have an Actor.
            3) ReferenceDialogue references dialogue lines. They are used to jump back in conversation.
            4) EndDialogue ends the dialogue.

        WARNING!!! There are no tests for the validation: it's likely to not be foolproof. DON'T blindly rely on it! */
    public DialogueTree ReadDialogueFile(string[] allLines)
    {
        _hasEndKeyword = false;
        _dialogueTree = new DialogueTree();
        _currentNode = _dialogueTree.Root;
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

            if (((_indentation - _prevIndentation) % 4 != 0) ||
                (_indentation > _prevIndentation && _indentation - _prevIndentation != 4))
            {
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': line {1}. Indentation in dialogue files should be exactly 4.", _filename, _lineNumber));
                break;
            }

            // OK, ready to read this line with regex.

            IDialogue dialogue;

            Match endMatch = _regexes["endRegex"].Match(trimmed);
            Match continueMatch = _regexes["continueRegex"].Match(trimmed);
            Match refMatch = _regexes["refRegex"].Match(trimmed);
            Match actorMatch = _regexes["actorRegex"].Match(trimmed);
            Match playerMatch = _regexes["playerRegex"].Match(trimmed);
            
            if (endMatch.Success)          // this line is an EndDialogue line
                dialogue = HandleEndDialogue(endMatch);
            else if (continueMatch.Success)     // this line is a ContinueDialogue line
                dialogue = new ContinueDialogue();
            else if (refMatch.Success)     // this line is a RefDialogue line
                dialogue = HandleRefDialogue(refMatch);
            else if (actorMatch.Success)   // this line is an ActorDialogue line
                dialogue = HandleActorDialogue(actorMatch);
            else if (playerMatch.Success)  // this line is a PlayerDialogue line
                dialogue = HandlePlayerDialogue(playerMatch);
            else                           // error, bad syntax in this line
            {
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': line {1} didn't match the dialogue syntax.", _filename, _lineNumber));
                continue;
            }

            // Place the dialogue line in the correct place inside DialogueTree.
            AddNodeToDialogueTree(dialogue);

            _lineNumber++;
        }

        if (!_hasEndKeyword)
        {
            Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': {END} command missing.", _filename));
            return null;
        }

        return _dialogueTree;
    }

    private void AddNodeToDialogueTree(IDialogue dialogue)
    {
        while (_currentNode.Parent != null &&
               _indentation <= _prevIndentation)
        {
            _prevIndentation -= 4;
            _currentNode = _currentNode.Parent;
        }
        
        _currentNode = _currentNode.AddChild(id: _lineNumber, dialogue);
    }

    private IDialogue HandleActorDialogue(Match match)
    {
        List<IDialogueAction> actions = SplitAndValidateActions(match.Groups[4].Value);
        List<IDialogueCondition> conditions = SplitAndValidateConditions(match.Groups[3].Value);

        return new ActorDialogue(
            match.Groups[1].Value,  // Actor
            match.Groups[2].Value,  // Line
            conditions,             // Conditions
            actions                 // Actions
        );
    }

    private IDialogue HandleEndDialogue(Match match)
    {
        _hasEndKeyword = true;
        return new EndDialogue();
    }

    // TO-DO: Some copy-paste here... cleanup
    private IDialogue HandlePlayerDialogue(Match match)
    {
        List<IDialogueAction> actions = SplitAndValidateActions(match.Groups[4].Value, match.Groups[1].Value);
        List<IDialogueCondition> conditions = SplitAndValidateConditions(match.Groups[3].Value);

        return new PlayerDialogue(
            match.Groups[1].Value,  // BodyPart
            match.Groups[2].Value,  // Line
            conditions,             // Conditions
            actions                 // Actions
        );
    }

    private IDialogue HandleRefDialogue(Match match)
    {
        int refLineNumber;
        int.TryParse(match.Groups[2].Value, out refLineNumber);

        // Does this Ref reference children?
        if (match.Groups[3].Success)
            return new RefDialogue(refLineNumber, refChildren: true);

        // Reference a single line
        return new RefDialogue(refLineNumber);
    }

    private List<IDialogueAction> SplitAndValidateActions(string actionsString, string bodyPart = "")
    {
        List<IDialogueAction> actions = new List<IDialogueAction>();

        if (string.IsNullOrWhiteSpace(actionsString)) return actions;

        // Split Actions into a list
        List<string> splitActions = actionsString.Split(',')
                                                 .Select(s => s.Trim())
                                                 .ToList();

        // Validate each Action with regex
        foreach (string action in splitActions)
        {
            Match setMatch = _regexes["setActionRegex"].Match(action);
            Match toggleMatch = _regexes["toggleActionRegex"].Match(action);
            Match addMatch = _regexes["addActionRegex"].Match(action);
            Match rollMatch = _regexes["rollActionRegex"].Match(action);

            if (setMatch.Success)
            {
                bool boolValue;
                bool.TryParse(setMatch.Groups[3].Value, out boolValue);

                actions.Add(new SetDialogueAction(setMatch.Groups[2].Value, boolValue));
            }
            else if (toggleMatch.Success)
            {
                actions.Add(new ToggleDialogueAction(toggleMatch.Groups[2].Value));
            }
            else if (addMatch.Success)
            {
                int val;
                int.TryParse(addMatch.Groups[3].Value, out val);

                actions.Add(new AddDialogueAction(addMatch.Groups[2].Value, val));
            }
            else if (rollMatch.Success)
            {
                Match bodyPartMatch = _regexes["bodyPartRegex"].Match(bodyPart);

                if (!bodyPartMatch.Success)
                {
                    Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': line {1}. Bad body part value in Actions syntax.", _filename, _lineNumber));
                    continue;
                }

                int numerator;
                int denominator;
                int.TryParse(rollMatch.Groups[2].Value, out numerator);
                int.TryParse(rollMatch.Groups[3].Value, out denominator);

                if (numerator <= denominator && numerator >= 0 && denominator > 0)
                    actions.Add(new RollDialogueAction(numerator, denominator));
                else
                    Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': line {1}. Bad dice roll values in Actions syntax.", _filename, _lineNumber));
            }
            else
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': line {1}. Bad Actions syntax.", _filename, _lineNumber));
        }

        return actions;
    }

    private List<IDialogueCondition> SplitAndValidateConditions(string conditionsString)
    {
        List<IDialogueCondition> conditions = new List<IDialogueCondition>();

        if (string.IsNullOrWhiteSpace(conditionsString)) return conditions;

        // Split Conditions into a list. Concatenate "IF " to all but the first Condition
        List<string> splitConditions = conditionsString.Split(new string[] { " AND " }, StringSplitOptions.None)
                                                       .Select((s, i) => i == 0 ? s.Trim() : "IF " + s.Trim())
                                                       .ToList();

        // Validate each Condition with regex
        foreach (string condition in splitConditions)
        {
            Match boolMatch = _regexes["boolConditionRegex"].Match(condition);
            Match intMatch = _regexes["intConditionRegex"].Match(condition);

            if (intMatch.Success)
            {
                bool negator = intMatch.Groups[2].Success;
                string variable = intMatch.Groups[3].Value;
                string op = intMatch.Groups[4].Value;
                int val;
                int.TryParse(intMatch.Groups[5].Value, out val);

                conditions.Add(new IntDialogueCondition(negator, variable, op, val));
            }
            else if (boolMatch.Success)
            {
                bool negator = boolMatch.Groups[2].Success;
                string variable = boolMatch.Groups[3].Value;

                conditions.Add(new BoolDialogueCondition(negator, variable));
            }
            else
                Debug.LogError(string.Format("DialogueError: Error when reading '{0}.dlg': line {1}. Bad Conditions syntax.", _filename, _lineNumber));
        }

        return conditions;
    }
}
