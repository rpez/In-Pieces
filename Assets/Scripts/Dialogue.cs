using System;
using System.Collections.Generic;
using System.Linq;

public interface IDialogue
{
    public string ToString(); 
}

public interface IConditionalDialogue : IDialogue
{
    public ICondition Condition { get; }
}

public class ActorDialogue : IConditionalDialogue
{
    public string Actor { get; }

    public string Line { get; }

    public ICondition Condition { get; }

    public List<string> Actions { get; }

    public ActorDialogue(string actor, string line, ICondition condition = null, List<string> actions = null)
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

    public ICondition Condition { get; }

    public List<string> Actions { get; }

    public PlayerDialogue(string bodyPart, string line, ICondition condition = null, List<string> actions = null)
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

public interface ICondition
{
    public string Variable { get; }
}

public class BoolCondition : ICondition
{
    public string Variable { get; }

    public BoolCondition(string variable)
    {
        Variable = variable;
    }
}

public class IntCondition : ICondition
{
    public string Variable { get; }
    public string Operator { get; }
    public int Value { get; }

    public IntCondition(string variable, string op, string valueAsString)
    {
        Operator = op;

        int val;
        int.TryParse(valueAsString, out val);
        Value = val;
    }
}
