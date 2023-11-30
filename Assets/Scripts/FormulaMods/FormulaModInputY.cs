using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaModInputY : FormulaMod
{
#if UNITY_EDITOR
    public override bool IsRemovable { get; } = false;
#endif

    [System.NonSerialized]
    private float m_value;

    public float Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_value;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => m_value = value;
#endif
    }

#if UNITY_EDITOR
    protected override void Initialize()
    {
        name = "Y [In]";
    }
#endif

    public override string VarPrefix => null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return m_value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        return $"y";
    }
}
