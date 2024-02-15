using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class AbsDrawer : BaseDrawer
{
    public override string HelpMessage => m_helpMessage;

    private const string m_helpMessage = "[<color=orange>|a|</color>] calculates the modulus from the input slot value.\n" +
        "Underlying formula will be like\n" +
        "        res = abs(a)\n" +
        "where 'a' is input slot parameter.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public AbsDrawer(FormulaModAbs mod)
        : base(mod) { }

    static AbsDrawer()
    {
        Register(typeof(FormulaModAbs), mod => new AbsDrawer((FormulaModAbs)mod));
    }
}
