using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class OneMinusDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>OneMinus</color>] subtracts first input slot value from 1.\n" +
        "Underlying formula will be like\n" +
        "        res = 1 - x\n" +
        "where 'x' is input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public OneMinusDrawer(FormulaModOneMinus mod)
        : base(mod) { }

    static OneMinusDrawer()
    {
        Register(typeof(FormulaModOneMinus), mod => new OneMinusDrawer((FormulaModOneMinus)mod));
    }
}
