using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSqrt : FormulaMod
{
#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "√a";

        await AddInput("Value");
    }
#endif

    private static readonly string m_varPrefix = "sqrt";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        var value = Inputs[0].CalculateInput();
        return value < 0
            ? float.NaN
            : Mathf.Sqrt(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>Mathf</color>.<color=#74531f>Sqrt</color>({val0});");
        }

        return VarName;
    }
}
