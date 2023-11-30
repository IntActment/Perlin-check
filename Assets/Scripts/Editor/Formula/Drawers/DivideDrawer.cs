using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class DivideDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>a / b</color>] divides the first input slot value by the second input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = a / b\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.\n" +
        "If 'b' is zero, it returns NaN.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DivideDrawer(FormulaModDivide mod)
        : base(mod) { }

    static DivideDrawer()
    {
        Register(typeof(FormulaModDivide), mod => new DivideDrawer((FormulaModDivide)mod));
    }
}
