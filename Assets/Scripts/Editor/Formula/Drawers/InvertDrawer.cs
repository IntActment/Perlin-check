using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class InvertDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Invert</color>] inverts the first input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = 1 / a\n" +
        "where 'a' is input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public InvertDrawer(FormulaModInvert mod)
        : base(mod) { }

    static InvertDrawer()
    {
        Register(typeof(FormulaModInvert), mod => new InvertDrawer((FormulaModInvert)mod));
    }
}
