using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class MultiplyDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>a * b</color>] multiplies the first input slot value by the second input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = a * b\n" +
        "where 'a' is the first input slot parameter and\n" +
        "      'b' is the second input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MultiplyDrawer(FormulaModMultiply mod)
        : base(mod) { }

    static MultiplyDrawer()
    {
        Register(typeof(FormulaModMultiply), mod => new MultiplyDrawer((FormulaModMultiply)mod));
    }
}
