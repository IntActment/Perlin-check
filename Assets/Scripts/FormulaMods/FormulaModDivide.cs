using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModDivide : FormulaMod
{
    [SerializeField]
    private float m_dividend = 1;

    public float Dividend
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_dividend;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_dividend, value);
#endif
    }

    [SerializeField]
    private float m_divisor = 1;

    public float Divisor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_divisor;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_divisor, value);
#endif
    }

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "a / b";

        await AddInput("Dividend", true);
        await AddInput("Divisor", true);
    }
#endif

    private static readonly string m_varPrefix = "div";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        float dividend = PickValue(0, m_dividend);
        float divisor = PickValue(1, m_divisor);

        return Mathf.Approximately(divisor, 0)
            ? float.PositiveInfinity
            : dividend / divisor;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = PickCode(0, m_dividend, vars, builder);
            var val1 = PickCode(1, m_divisor, vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = {val0} / {val1};");
        }

        return VarName;
    }
}
