using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class InputYDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "<color=orange>Y [In]</color>] is the 'y' input value provider. Cannot be removed.";

    public override bool HasInput { get; } = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InputYDrawer(FormulaModInputY mod)
        : base(mod) { }

    static InputYDrawer()
    {
        Register(typeof(FormulaModInputY), mod => new InputYDrawer((FormulaModInputY)mod));
    }
}
