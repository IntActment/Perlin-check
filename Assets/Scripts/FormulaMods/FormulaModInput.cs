using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public class FormulaModInput : FormulaMod
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return m_value;
    }
}
