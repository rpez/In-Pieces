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

        foreach (string line in File.ReadLines(filename))
        {
            if (string.IsNullOrWhiteSpace(line)) continue; // empty line

            string trimmed = line.Trim();

            if (trimmed.Substring(0, 1) == "#") continue; // comment line

            if (trimmed.Substring(0, 1) == "{") // command line
            {
                if (trimmed.Equals("{END}")) hasEndKeyword = true;
            }
            else // dialogue line
            {
                // 1st capture group: the speaker
                // 2nd capture group: the line
                // 3rd capture group: command or condition
                Regex regex = new Regex(@"^(\w+[ :])+ (\w+[ ,.!'?] ?)+(\{.*\})?$", RegexOptions.Singleline);

                Match match = regex.Match(trimmed);
            }
        }

        if (!hasEndKeyword)
        {
            // should error here, dialogue file is bad
        }
    }
}
