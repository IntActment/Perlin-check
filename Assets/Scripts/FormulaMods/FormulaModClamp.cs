using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModClamp : FormulaMod
{
    [SerializeField]
    private float m_min = 0;

    public float Min
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_min;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_min, value);
#endif
    }

    [SerializeField]
    private float m_max = 1;

    public float Max
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_max;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_max, value);
#endif
    }

    protected override void OnEnable()
    {
        name = "Clamp";
    }

#if UNITY_EDITOR
    protected override void Initialize()
    {
        AddInput("Value");
    }
#endif

    private static readonly string m_varPrefix = "clamp01";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.Clamp(Inputs[0].CalculateInput(), m_min, m_max);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>Mathf</color>.<color=#74531f>Clamp</color>({val0}, {m_min}f, {m_max}f);");
        }

        return VarName;
    }
}
