using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class LerpDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>Lerp</color>] linearly interpolates between the first and the second input slot\n" +
        "  values with delta value provided with the third input slot.\n" +
        "Underlying formula will be like\n" +
        "        res = a + delta * (b - a)\n" +
        "where 'a' is  the first input slot parameter,\n" +
        "      'b' is the second input slot parameter and\n" +
        "      'delta' is the third input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LerpDrawer(FormulaModLerp mod)
        : base(mod) { }

    static LerpDrawer()
    {
        Register(typeof(FormulaModLerp), mod => new LerpDrawer((FormulaModLerp)mod));
    }
}
