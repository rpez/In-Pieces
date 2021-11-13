using System;
using System.Collections.Generic;

public class RefDialogue : Dialogue
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
