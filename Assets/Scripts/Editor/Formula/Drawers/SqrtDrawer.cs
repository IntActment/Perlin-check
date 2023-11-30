using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class SqrtDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>√a</color>] calculates the square root from the input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = sqrt(a)\n" +
        "where 'a' is input slot parameter.\n" +
        "If 'a' is negative, it returns NaN";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SqrtDrawer(FormulaModSqrt mod)
        : base(mod) { }

    static SqrtDrawer()
    {
        Register(typeof(FormulaModSqrt), mod => new SqrtDrawer((FormulaModSqrt)mod));
    }
}
