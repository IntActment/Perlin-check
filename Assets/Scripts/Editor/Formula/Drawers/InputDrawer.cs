using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class InputDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Input</color>] is the input value provider. Cannot be removed.";

    public override bool HasInput { get; } = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputDrawer(FormulaModInput mod)
        : base(mod) { }

    static InputDrawer()
    {
        Register(typeof(FormulaModInput), mod => new InputDrawer((FormulaModInput)mod));
    }
}
