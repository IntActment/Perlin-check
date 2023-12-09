using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaSocketOut : FormulaSocket
{
    [field: NonSerialized]
    public override FormulaSocketType SocketType { get; } = FormulaSocketType.Out;

    [SerializeField]
    private FormulaSocketIn m_link;

    public FormulaSocketIn Link
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
    protected override async Task OnLateInit()
    {
        await Task.CompletedTask;
    }
#endif
}
