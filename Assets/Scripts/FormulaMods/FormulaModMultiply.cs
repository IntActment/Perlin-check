using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModMultiply : FormulaMod
{
    [SerializeField]
    private float m_multiplicand = 1;

    public float Multiplicand
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_multiplicand;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_multiplicand, value);
#endif
    }

    [SerializeField]
    private float m_multiplier = 1;

    public float Multiplier
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_multiplier;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_multiplier, value);
#endif
    }

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "a * b";

        await AddInput("Multiplicand", true);
        await AddInput("Multiplier", true);
    }
#endif

    private static readonly string m_varPrefix = "mul";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return PickValue(0, m_multiplicand) * PickValue(1, m_multiplier);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = PickCode(0, m_multiplicand, vars, builder);
            var val1 = PickCode(1, m_multiplier, vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = {val0} * {val1};");
        }

        return VarName;
    }
}
