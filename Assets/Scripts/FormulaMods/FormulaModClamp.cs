using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "Clamp";

        await AddInput("Value");
    }

    protected override async Task OnLateAwake()
    {
        if (m_inputs.Count < 2)
        {
            await AddInput("Min", true);
        }

        if (m_inputs.Count < 3)
        {
            await AddInput("Max", true);
        }
    }
#endif

    private static readonly string m_varPrefix = "clamp01";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.Clamp(Inputs[0].CalculateInput(), PickValue(1, m_min), PickValue(2, m_max));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>Mathf</color>.<color=#74531f>Clamp</color>({val0}, {PickCode(1, m_min, vars, builder)}, {PickCode(2, m_max, vars, builder)});");
        }

        return VarName;
    }
}
