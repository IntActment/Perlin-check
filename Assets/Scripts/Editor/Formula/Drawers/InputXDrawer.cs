using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class InputXDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "<color=orange>X [In]</color> is the 'x' input value provider. Cannot be removed.";

    public override bool HasInput { get; } = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputXDrawer(FormulaModInputX mod)
        : base(mod) { }

    static InputXDrawer()
    {
        Register(typeof(FormulaModInputX), mod => new InputXDrawer((FormulaModInputX)mod));
    }
}
