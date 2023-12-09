using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public abstract class FormulaSocket : ComplexScriptable
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

#if UNITY_EDITOR
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnDisable()
    {
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
    }
#endif
}
