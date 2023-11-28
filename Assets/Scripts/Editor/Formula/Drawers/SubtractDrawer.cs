using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class SubtractDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Subtract</color>] subtracts the second input slot value from the first input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = a - b\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SubtractDrawer(FormulaModSubtract mod)
        : base(mod) { }

    static SubtractDrawer()
    {
        Register(typeof(FormulaModSubtract), mod => new SubtractDrawer((FormulaModSubtract)mod));
    }
}
