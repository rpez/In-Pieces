using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser
{

    private bool _hasEndKeyword;  // makes sure there is an escape from this conversation
    private DialogueTree _dialogueTree;
    private TreeNode<Dialogue> _currentNode;
    private int _lineNumber;
    private int _indentation;
    private int _prevIndentation;
    private string _filename;

    public DialogueParser()
    {
        DialogueTree dialogueTree = ReadFile("sketch_dialogue_1.dlg");

        /*Debug.Log("Value: " + dialogueTree.Root.Value + ", Id: " + dialogueTree.Root.Id);
        Debug.Log(dialogueTree.Root.Children);
        Debug.Log("Value: " + dialogueTree.Root.Children[0].Value + ", Id: " + dialogueTree.Root.Children[0].Id);
        Debug.Log("Value: " + dialogueTree.Root.Children[1].Value + ", Id: " + dialogueTree.Root.Children[1].Id);*/
    }

    /*  Read lines from a dialogue file one by one.
        Creates a single DialogueTree and populates it with the lines read from the dialogue file as Dialogue classes.

        There exists four Dialogue classes:

            1) PlayerDialogue is spoken by the player. It has no Actor.
            2) ActorDialogue is spoken by NPCs. They have an Actor.
            3) ReferenceDialogue references dialogue lines. They are used to jump back in conversation, or end the conversation.
            4) EndDialogue ends the dialogue.

        WARNING!!! The validation has not been tested and therefore is likely to not be foolproof. DON'T blindly rely on it! */
    private DialogueTree ReadFile(string filename)
    {
        _hasEndKeyword = false;
        _dialogueTree = null;
        _currentNode = null;
        _lineNumber = 1;
        _indentation = 0;
        _filename = filename;

        // Dialogue files should be located in "Assets/Dialogue/"
        string path = Path.Combine(Application.dataPath, "Dialogue/", _filename);

        // Regexes that we will need
        Regex refRegex = new Regex(@"\{(?:(LINE) (\d+)(?: (CHILDREN)?)|END)\}");
        Regex dialogueRegex = new Regex(@"^((?:[\w ]+)*)(?:: |\d\. )([\w ,.!'""-?]+) ?(\{?.*\})? ?(\[.*\])?$", RegexOptions.Singleline);

        foreach (string line in File.ReadLines(path))
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // this line is empty, skip it

            string trimmed = line.Trim();

            if (trimmed.Substring(0, 1) == "#") continue; // this line is a comment, skip it

            // Count indentation. Accept only spaces, no tabs.
            _prevIndentation = _indentation;
            _indentation = line.TakeWhile(c => c.Equals(' ')).Count();

            if ((_indentation - _prevIndentation) % 4 != 0)
            {
                Debug.LogError(string.Format("DialogueError: Error in '{0}': Indentation in dialogue files should be 4.", _filename));
                break;
            }

            // OK, ready to read this line with regex.

            Dialogue dialogue;

            Match refMatch = refRegex.Match(trimmed);
            Match dialogueMatch = dialogueRegex.Match(trimmed);

            if (refMatch.Success)            // this line is a reference to other dialogue lines, handle it differently
                dialogue = HandleRefDialogue(refMatch);
            else if (dialogueMatch.Success)  // this line is a normal dialogue line
            {
                dialogue = HandleNormalDialogue(dialogueMatch);

                if (dialogue == null) continue;
            }
            else                             // error, bad syntax in this line
            {
                Debug.LogError(string.Format("DialogueError: Error in '{0}': Line " + _lineNumber + " didn't match the dialogue syntax.", _filename));
                continue;
            }

            // Place the dialogue line in the correct place inside DialogueTree.
            AddNodeToDialogueTree(dialogue);

            _lineNumber++;
        }

        if (!_hasEndKeyword)
        {
            Debug.LogError(string.Format("DialogueError: Error in '{0}': {END} command missing.", _filename));
            return null;
        }

        return _dialogueTree;
    }

    private void AddNodeToDialogueTree(Dialogue dialogue)
    {
        if (_dialogueTree == null)  // Create a new DialogueTree if one doesn't exist yet.
        {
            _dialogueTree = new DialogueTree(new TreeNode<Dialogue>(id: _lineNumber, dialogue));
            _currentNode = _dialogueTree.Root;
        }
        else if (_indentation > _prevIndentation)   // We went one level lower in the tree. Add child and set it to be the current node.
        {
            _currentNode = _currentNode.AddChild(id: _lineNumber, dialogue);
        }
        else if (_indentation == _prevIndentation)  // We are at the same level in the tree. Add child, don't touch the current node.
        {
            _currentNode.AddChild(id: _lineNumber, dialogue);
        }
        else                                      // We are n levels higher in the tree. Go up as much as we have to, then add child and set it to be the current node.
        {
            while (_currentNode.Parent != null && _indentation <= _prevIndentation)
            {
                _prevIndentation -= 4;
                _currentNode = _currentNode.Parent;
            }
            _currentNode = _currentNode.AddChild(id: _lineNumber, dialogue);
        }
    }

    private Dialogue HandleRefDialogue(Match match)
    {
        if (match.Value.Equals("{END}"))
        {
            _hasEndKeyword = true;
            return new EndDialogue();
        }

        int refLineNumber;
        int.TryParse(match.Groups[2].Value, out refLineNumber);

        if (match.Groups[3].Value == " CHILDREN")
        {
            return new RefDialogue(refLineNumber);
        }

        return new RefDialogue(refLineNumber - 1);
    }

    // Returns either PlayerDialogue or ActorDialogue based on the regex match.
    private Dialogue HandleNormalDialogue(Match match)
    {
        // First split actions into a list
        List<string> actions = null;

        if (!string.IsNullOrWhiteSpace(match.Groups[4].Value))
        {
            try
            {
                actions = match.Groups[4].Value
                    .Substring(1, match.Groups[4].Value.Length - 2)  // remove first and last character []
                    .Split(',')                                      // split on comma
                    .ToList();
            }
            catch
            {
                Debug.LogError(string.Format("DialogueError: Error in '{0}': Line " + _lineNumber + " has problems with its Actions syntax.", _filename));
                return null;
            }
        }

        if (string.IsNullOrWhiteSpace(match.Groups[1].Value))
        {
            return new PlayerDialogue(
                match.Groups[2].Value,  // Line
                match.Groups[3].Value,  // Condition
                actions                         // Actions
            );
        }

        return new ActorDialogue(
            match.Groups[1].Value,  // Actor
            match.Groups[2].Value,  // Line
            match.Groups[3].Value,  // Condition
            actions                         // Actions
        );
    }
}
