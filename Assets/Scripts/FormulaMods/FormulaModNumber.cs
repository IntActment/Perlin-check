using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModNumber : FormulaMod
{
    [SerializeField]
    private float m_value;

    public float Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_value;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_value, value);
#endif
    }

#if UNITY_EDITOR
    protected override void Initialize()
    {
        name = "Number";
    }
#endif

    private static readonly string m_varPrefix = "num";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            builder.AppendLine($"        <color=blue>const float</color> {VarName} = 1 - {m_value}f;");
        }

        return VarName;
    }
}
