using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public abstract class FormulaSocket : ScriptableObject
{
    [SerializeField]
    private FormulaMod m_owner;

    public FormulaMod Owner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_owner;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.SetValue(ref m_owner, value);
    }

    public abstract FormulaSocketType SocketType { get; }
}
