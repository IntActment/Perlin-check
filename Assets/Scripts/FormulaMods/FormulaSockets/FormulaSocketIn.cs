using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public class FormulaSocketIn : FormulaSocket
{
    [field: NonSerialized]
    public override FormulaSocketType SocketType { get; } = FormulaSocketType.In;

    [SerializeField]
    private FormulaSocketOut m_link;

    public FormulaSocketOut Link
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_link;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (this.SetValue(ref m_link, value))
            {
                Owner?.Formula?.InvokeChanged();
            }
        }
#endif
    }

#if UNITY_EDITOR
    [SerializeField]
    private string m_title = string.Empty;

    public string Title
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_title;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.SetValue(ref m_title, value);
    }
#endif

#if UNITY_EDITOR
    [SerializeField]
    private bool m_isOptional = false;

    public bool IsOptional
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_isOptional;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.SetValue(ref m_isOptional, value);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float CalculateInput()
    {
        return (null == Link)
            ? 0
            : Link.Owner.Calculate(Owner.Formula.CalcCompletitionList, Owner.Formula.CalcValuesList);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        return (null == Link)
            ? "0"
            : Link.Owner.GenerateCode(vars, builder);
    }
}
