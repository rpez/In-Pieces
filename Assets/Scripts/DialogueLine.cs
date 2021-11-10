using System;

public class Dialogue
{
    private readonly string _actor;
    private readonly string _line;
    private readonly string _command;
    private readonly string _bonus;

    public string Actor { get { return _actor; } }

    public string Line { get { return _line; } }

    public string Command { get { return _command; } }

    public string Bonus { get { return _bonus; } }

    public Dialogue(string actor, string line, string command = null, string bonus = null)
    {
        _actor = actor;
        _line = line;
        _command = command;
    }
}
