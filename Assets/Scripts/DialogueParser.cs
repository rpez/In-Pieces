using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueParser
{
    public DialogueParser()
    {

    }

    private void ReadFile(string filename)
    {
        // make sure there is an escape from this conversation
        bool hasEndKeyword = false;

        TreeNode<Dialogue> dialogueTree;
        TreeNode<Dialogue> currentNode;

        int i = 1;
        int indentation = 0;

        foreach (string line in File.ReadLines(filename))
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // empty line

            // Count indentation. Accept only spaces, no tabs.
            int prevIndentation = indentation;
            int indentation = line.TakeWhile(c => c.Equals(' ')).Count();

            if ((indentation - prevIndentation) % 4 != 0)
            {
                Debug.LogError("DialogueFileError: Indentation in dialogue files should be 4.");
                break;
            }

            string trimmed = line.Trim();

            if (trimmed.Substring(0, 1) == "#") continue; // comment

            if (trimmed.Substring(0, 1) == "{") // command
            {
                if (trimmed.Equals("{END}")) hasEndKeyword = true;
            }
            else // dialogue
            {
                // 1st capture group: the speaker
                // 2nd capture group: the line
                // 3rd capture group: command or condition
                // 4th capture group: bonuses or penalties
                Regex regex = new Regex(@"^([\w ]+)+: ([\w ,.!'?]+) ?(\{?.*\})? ?(\[.*\])?$", RegexOptions.Singleline);
                Match match = regex.Match(trimmed);

                if (!match.Success)
                {
                    Console.LogError("DialogueError: Line " + i + " didn't match the dialogue syntax.");
                }

                Dialogue dialogue = new Dialogue(match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]);

                if (!dialogueTree)
                {
                    dialogueTree = new TreeNode<>(dialogueLine);
                    currentNode = dialogueTree;
                }
                else if (indentation > prevIndentation)
                {
                    currentNode = dialogueTree.AddChild();
                }
                else if (indentation == prevIndentation)
                {
                    dialogueTree.AddChild();
                }
                else
                {
                    while (indentation <= prevIndentation)
                    {
                        prevIndentation -= 4;
                        currentNode = currentNode.Parent;
                    }
                    currentNode = dialogueTree.AddChild();
                }
            }

            i++;
        }

        if (!hasEndKeyword)
        {
            Debug.LogError("DialogueError: {END} command missing in dialogue file.")
        }
    }
}
