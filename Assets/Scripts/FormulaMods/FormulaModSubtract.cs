using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSubtract : FormulaMod
{
#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "a - b";

        await AddInput("Subtrahend");
        await AddInput("Minuend");
    }
#endif

    private static readonly string m_varPrefix = "sub";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Inputs[0].CalculateInput() - Inputs[1].CalculateInput();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = Inputs[1].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = {val0} - {val1};");
        }

        return VarName;
    }
}
