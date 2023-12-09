using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModNorm01 : FormulaMod
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

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "Norm01";

        await AddInput("Value");
    }
#endif

    private static readonly string m_varPrefix = "norm01";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        float range = m_max - m_min;
        if (Mathf.Approximately(0, range))
        {
            // preventing division by zero
            return 0;
        }

        return (Inputs[0].CalculateInput() - m_min) / range;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            float range = m_max - m_min;
            if (Mathf.Approximately(0, range))
            {
                // preventing division by zero
                builder.AppendLine($"        <color=blue>const float</color> {VarName} = 0;");
            }
            else
            {
                var val0 = Inputs[0].GenerateCode(vars, builder);

                builder.AppendLine($"        <color=blue>float</color> {VarName} = ({val0} - {m_min}f) / {range}f;");
            }
        }

        return VarName;
    }
}
