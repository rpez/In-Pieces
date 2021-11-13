using System;
using System.Collections.Generic;

public class ActorDialogue : Dialogue
{
    private readonly string _actor;
    private readonly string _line;
    private readonly string _condition;
    private readonly List<string> _actions;

    public string Actor { get => _actor; }

    public string Line { get => _line; }

    public string Condition { get => _condition; }

    public List<string> Actions { get => _actions; }

    public ActorDialogue(string actor, string line, string condition = null, List<string> actions = null)
    {
        _actor = actor;
        _line = line;
        _condition = condition;
        _actions = actions;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}", _actor, _line);
    }
}
