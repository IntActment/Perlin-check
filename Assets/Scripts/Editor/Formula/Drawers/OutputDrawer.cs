using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class OutputDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Output</color>] is the output value receiver. Cannot be removed.";
   
    public override bool HasOutput { get; } = false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OutputDrawer(FormulaModOutput mod)
        : base(mod) { }

    static OutputDrawer()
    {
        Register(typeof(FormulaModOutput), mod => new OutputDrawer((FormulaModOutput)mod));
    }
}
