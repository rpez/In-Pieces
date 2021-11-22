using System;
using System.Collections.Generic;
using System.Linq;

// Generic interface for all Dialogue
public interface IDialogue
{
    public string ToString();
}

/* Represents Dialogue that has:
    1) Conditions    if these are met, make the Dialogue appear to the player
    2) Actions       these can modify the game state */
public interface IConditionalDialogue : IDialogue
{
    public List<IDialogueCondition> Conditions { get; }
    public List<IDialogueAction> Actions { get; }
}

// Represents Dialogue that the player can select in the conversation screen
public interface ISelectableDialogue : IDialogue {}

public class ActorDialogue : IConditionalDialogue
{
    public string Actor { get; }

    public string Line { get; }

    public List<IDialogueCondition> Conditions { get; }

    public List<IDialogueAction> Actions { get; }

    public ActorDialogue(string actor, string line, List<IDialogueCondition> conditions = null, List<IDialogueAction> actions = null)
    {
        Actor = actor;
        Line = line;
        Conditions = conditions;
        Actions = actions;
    }

    public override string ToString()
    {
        return string.Format("{0}: {1}", Actor, Line);
    }
}

public class ContinueDialogue : ISelectableDialogue
{
    public override string ToString()
    {
        return "Continue";
    }
}

public class EndDialogue : ISelectableDialogue
{
    public override string ToString()
    {
        return "End Dialogue";
    }
}

public class PlayerDialogue : IConditionalDialogue, ISelectableDialogue
{
    public string BodyPart { get; }

    public string Line { get; }

    public List<IDialogueCondition> Conditions { get; }

    public List<IDialogueAction> Actions { get; }

    public PlayerDialogue(string bodyPart, string line, List<IDialogueCondition> conditions = null, List<IDialogueAction> actions = null)
    {
        Line = line;
        BodyPart = bodyPart;
        Conditions = conditions;
        Actions = actions;
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(BodyPart) ? Line : string.Format("<{0}> {1}", BodyPart, Line);
    }
}

public class RefDialogue : IDialogue
{
    public int Ref { get; }
    public bool RefChildren { get; }

    public RefDialogue(int lineNumber, bool refChildren = false)
    {
        Ref = lineNumber;
        RefChildren = refChildren;
    }

    public override string ToString()
    {
        return RefChildren ? string.Format("{{REF {0} CHILDREN}}", Ref) : string.Format("{{REF {0}}}", Ref);
    }
}

public class RootDialogue : IDialogue
{
    public override string ToString()
    {
        return "{ROOT}";
    }
}

/* Dialogue Conditions */
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

/* Dialogue Actions */
public interface IDialogueAction
{
    public string ToString();
}

public class SetDialogueAction : IDialogueAction
{
    public string Variable { get; }
    public bool Value { get; }

    public SetDialogueAction(string variable, bool val)
    {
        Variable = variable;
        Value = val;
    }

    public override string ToString() => string.Format("SET {0} {1}", Variable, Value);
}

public class AddDialogueAction : IDialogueAction
{
    public string Variable { get; }
    public int Value { get; }

    public AddDialogueAction(string variable, int val)
    {
        Variable = variable;
        Value = val;
    }

    public override string ToString() => string.Format("ADD {0} {1}", Variable, Value);
}

public class RollDialogueAction : IDialogueAction
{
    public int Numerator { get; }
    public int Denominator { get; }

    public RollDialogueAction(int numerator, int denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }

    public override string ToString() => string.Format("ROLL {0}/{1}", Numerator, Denominator);
}
