using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModPow : FormulaMod
{
    [SerializeField]
    private float m_power = 2;

    public float Power
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_power;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_power, Mathf.Max(0, value));
#endif
    }

#if UNITY_EDITOR

    protected override void Initialize()
    {
        AddInput("Value");
    }
#endif

    protected override void OnEnable()
    {
        name = "Pow";
    }

    private static readonly string m_varPrefix = "pow";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.Pow(Inputs[0].CalculateInput(), m_power);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>Mathf</color>.<color=#74531f>Pow</color>({val0}, {m_power}f);");
        }

        return VarName;
    }
}
