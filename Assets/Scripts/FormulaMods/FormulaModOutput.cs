using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModOutput : FormulaMod
{
#if UNITY_EDITOR
    public override bool IsRemovable { get; } = false;

    protected override void Initialize()
    {
        AddInput("Result");
    }
#endif

    protected override void OnEnable()
    {
        name = "[Out]";
    }

    public override string VarPrefix => null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return Inputs[0].CalculateInput();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        var val0 = Inputs[0].GenerateCode(vars, builder);

        builder.AppendLine();
        builder.Append($"        <color=blue>return</color> {val0};");

        return null;
    }
}
