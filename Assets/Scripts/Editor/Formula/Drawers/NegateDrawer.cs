using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class NegateDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Negate</color>] negates the first input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = -x\n" +
        "where 'x' is input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NegateDrawer(FormulaModNegate mod)
        : base(mod) { }

    static NegateDrawer()
    {
        Register(typeof(FormulaModNegate), mod => new NegateDrawer((FormulaModNegate)mod));
    }
}
