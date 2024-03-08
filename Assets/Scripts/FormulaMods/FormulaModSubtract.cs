using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSubtract : FormulaMod
{
    [SerializeField]
    private float m_subtrahend = 0;

    public float Subtrahend
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_subtrahend;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_subtrahend, value);
#endif
    }

    [SerializeField]
    private float m_minuend = 0;

    public float Minuend
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_minuend;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_minuend, value);
#endif
    }

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "a - b";

        await AddInput("Subtrahend", true);
        await AddInput("Minuend", true);
    }
#endif

    private static readonly string m_varPrefix = "sub";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return PickValue(0, m_subtrahend) - PickValue(1, m_minuend);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = PickCode(0, m_subtrahend, vars, builder);
            var val1 = PickCode(1, m_minuend, vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = {val0} - {val1};");
        }

        return VarName;
    }
}
