using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModMax : FormulaMod
{
    [SerializeField]
    private float m_value2 = 0;

    public float Value2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_value2;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_value2, value);
#endif
    }

#if UNITY_EDITOR

    protected override async Task Initialize()
    {
        name = "max";

        await AddInput("Value 1");
        await AddInput("Value 2", true);
    }
#endif

    private static readonly string m_varPrefix = "max";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Mathf.Max(Inputs[0].CalculateInput(), PickValue(1, m_value2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = PickCode(1, m_value2, vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>Mathf</color>.<color=#74531f>Max</color>({val0}, {val1});");
        }

        return VarName;
    }
}
