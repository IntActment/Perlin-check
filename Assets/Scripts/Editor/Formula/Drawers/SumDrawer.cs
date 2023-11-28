using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class SumDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Sum</color>] summarizes the first input slot value with the second input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = a + b\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SumDrawer(FormulaModSum mod)
        : base(mod) { }

    static SumDrawer()
    {
        Register(typeof(FormulaModSum), mod => new SumDrawer((FormulaModSum)mod));
    }
}
