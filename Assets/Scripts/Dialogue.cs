using System;
using System.Collections.Generic;
using System.Linq;

public interface IDialogue
{
    public string ToString();
}

public interface IConditionalDialogue : IDialogue
{
    public IDialogueCondition Condition { get; }
}

public class ActorDialogue : IConditionalDialogue
{
    public string Actor { get; }

    public string Line { get; }

    public IDialogueCondition Condition { get; }

    public List<string> Actions { get; }

    public ActorDialogue(string actor, string line, IDialogueCondition condition = null, List<string> actions = null)
    {
        Actor = actor;
        Line = line;
        Condition = condition;
        Actions = actions;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}", Actor, Line);
    }
}

public class EndDialogue : IDialogue
{
    public override string ToString()
    {
        return "End Dialogue";
    }
}

public class PlayerDialogue : IConditionalDialogue
{
    public string BodyPart { get; }

    public string Line { get; }

    public IDialogueCondition Condition { get; }

    public List<string> Actions { get; }

    public PlayerDialogue(string bodyPart, string line, IDialogueCondition condition = null, List<string> actions = null)
    {
        Line = line;
        BodyPart = bodyPart;
        Condition = condition;
        Actions = actions;
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(BodyPart) ? Line : string.Format("<{0}> {1}", BodyPart, Line);
    }
}

public class RefDialogue : IDialogue
{
    private readonly int _lineNumber;

    public int Ref { get => _lineNumber; }

    public RefDialogue(int lineNumber)
    {
        _lineNumber = lineNumber;
    }

    public override string ToString()
    {
        return string.Format("{{REF {0}}}", _lineNumber);
    }
}

public interface IDialogueCondition
{
    public bool Negator { get; }
    public string Variable { get; }
    public string ToString();
}

public class BoolDialogueCondition : IDialogueCondition
{
    public bool Negator { get; }
    public string Variable { get; }

    public BoolDialogueCondition(bool negator, string variable)
    {
        Negator = negator;
        Variable = variable;
    }

    public override string ToString() => Negator ? string.Format("NOT {0}", Variable) : Variable;
}

public class IntDialogueCondition : IDialogueCondition
{
    public bool Negator { get; }
    public string Variable { get; }
    public string Operator { get; }
    public int Value { get; }

    public IntDialogueCondition(bool negator, string variable, string op, int val)
    {
        Negator = negator;
        Variable = variable;
        Operator = op;
        Value = val;
    }

    public override string ToString() => Negator ? "NOT" : "" + string.Format("{0} {1} {2}", Variable, Operator, Value);
}
