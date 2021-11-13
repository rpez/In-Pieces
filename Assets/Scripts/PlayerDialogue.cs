using System;
using System.Collections.Generic;

public class PlayerDialogue : Dialogue
{
    private readonly string _line;
    private readonly string _condition;
    private readonly List<string> _actions;

    // Should we later replace this by getting PLAYER_NAME from GameState?
    public string Actor { get => null; }

    public string Line { get => _line; }

    public string Condition { get => _condition; }

    public List<string> Actions { get => _actions; }

    public PlayerDialogue(string line, string condition = null, List<string> actions = null)
    {
        _line = line;
        _condition = condition;
        _actions = actions;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}", _line);
    }
}
