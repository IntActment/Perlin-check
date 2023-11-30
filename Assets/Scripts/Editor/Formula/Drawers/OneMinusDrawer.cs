using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class OneMinusDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>1 - a</color>] subtracts first input slot value from 1.\n" +
        "Underlying formula will be like\n" +
        "        res = 1 - a\n" +
        "where 'a' is input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OneMinusDrawer(FormulaModOneMinus mod)
        : base(mod) { }

    static OneMinusDrawer()
    {
        Register(typeof(FormulaModOneMinus), mod => new OneMinusDrawer((FormulaModOneMinus)mod));
    }
}
