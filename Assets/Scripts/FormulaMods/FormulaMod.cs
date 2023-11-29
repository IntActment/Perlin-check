using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;

public abstract class FormulaMod : ComplexScriptable
{
#if UNITY_EDITOR

    [SerializeField]
    private Vector2 m_position;

    public Vector2 Position
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_position;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => DirtySetValue(ref m_position, value);
    }

    [SerializeField]
    private ComplexFormula m_formula;

    public ComplexFormula Formula
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_formula;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            this.SetValue(ref m_formula, value);

            if (null != value)
            {
                // find free index number
                var mods = Formula.Modifiers.ToList();
                bool hasDup;
                int index = 0;

                while (mods.Count > 0)
                {
                    hasDup = false;

                    for (int i = 0; i < mods.Count; i++)
                    {
                        if (mods[i] == this)
                        {
                            mods.RemoveAt(i);
                            i--;
                            continue;
                        }

                        // ignore inputs/output mods
                        if (mods[i].VarPrefix == null)
                        {
                            mods.RemoveAt(i);
                            i--;
                            continue;
                        }

                        if (index == mods[i].m_varIndex)
                        {
                            hasDup = true;
                            mods.RemoveAt(i);
                            break;
                        }
                    }

                    if (false == hasDup)
                    {
                        break;
                    }

                    index++;
                }

                this.SetValue(ref m_varIndex, index);
            }
        }
    }
#endif

    [SerializeField]
    protected List<FormulaSocketIn> m_inputs = new List<FormulaSocketIn>();

    public IReadOnlyList<FormulaSocketIn> Inputs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_inputs;
        //private set => this.SetValue<List<FormulaSocketIn>>(ref m_inputs, value);
    }

    [SerializeField]
    protected List<FormulaSocketOut> m_outputs = new List<FormulaSocketOut>();

    public IReadOnlyList<FormulaSocketOut> Outputs
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_outputs;
        //private set => this.SetValue<List<FormulaSocketOut>>(ref m_outputs, value);
    }

    public abstract string VarPrefix { get; }

    [SerializeField]
    protected int m_varIndex = 0;

    public int VarIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_varIndex;
    }

    public string VarName
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => $"{VarPrefix}_{m_varIndex}";
    }

#if UNITY_EDITOR
    [field: NonSerialized]
    public virtual bool IsRemovable { get; } = true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void AddInput(string title)
    {
        var newSocket = CreateInstance<FormulaSocketIn>();
        newSocket.Owner = this;
        newSocket.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        newSocket.name = "Socket [In]";
        newSocket.Title = title;
        m_inputs.Add(newSocket);
        AddSubAsset(newSocket);
        this.Save();
    }

    protected virtual void OnEnable()
    {

    }

    [SerializeField]
    private bool m_isInit = false;

    public bool IsInitialized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_isInit;
    }

    protected sealed override void OnLateInit()
    {
        if (true == m_isInit)
        {
            return;
        }

        Initialize();

        m_isInit = true;
        this.Save();
    }

    protected virtual void Initialize()
    {

    }

    private bool CheckRecursion(FormulaMod mod)
    {
        if (this == mod)
        {
            return false;
        }

        for (int i = 0; i < Inputs.Count; i++)
        {
            var input = Inputs[i];
            if (input.Link == null)
            {
                continue;
            }

            if (false == input.Link.Owner.CheckRecursion(mod))
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool AddOutput(FormulaSocketIn targetSocketIn)
    {
        if (false == CheckRecursion(targetSocketIn.Owner))
        {
            return false;
        }

        var newSocket = CreateInstance<FormulaSocketOut>();
        newSocket.Link = targetSocketIn;
        newSocket.Owner = this;
        newSocket.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
        newSocket.name = "Socket [Out]";

        m_outputs.Add(newSocket);
        AddSubAsset(newSocket);
        this.Save();

        if (null != targetSocketIn.Link)
        {
            // remove old connection
            var owner = targetSocketIn.Link.Owner;
            owner.m_outputs.Remove(targetSocketIn.Link);
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(targetSocketIn.Link);
            owner.Save();
        }

        targetSocketIn.Link = newSocket;
        targetSocketIn.Save();

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReplaceOutput(int index, FormulaSocketIn targetSocketIn)
    {
        if (false == CheckRecursion(targetSocketIn.Owner))
        {
            return false;
        }

        if (index >= Outputs.Count)
        {
            AddOutput(targetSocketIn);
            return true;
        }

        var socket = Outputs[index];
        var oldLink = socket.Link;
        socket.Link = targetSocketIn;
        socket.Save();

        if (null != oldLink)
        {
            oldLink.Link = null;
        }

        if (null != targetSocketIn.Link)
        {
            // remove old connection
            var owner = targetSocketIn.Link.Owner;
            owner.m_outputs.Remove(targetSocketIn.Link);
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(targetSocketIn.Link);
            owner.Save();
        }

        targetSocketIn.Link = socket;
        targetSocketIn.Save();

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveOutput(FormulaSocketOut mySocketOut)
    {
        if (this != mySocketOut.Owner)
        {
            Debug.LogWarning("Can pass only socket that belongs to this Mod");
            return;
        }

        if (null != mySocketOut.Link)
        {
            // remove old connection
            mySocketOut.Link.Link = null;
        }

        m_outputs.Remove(mySocketOut);
        UnityEditor.AssetDatabase.RemoveObjectFromAsset(mySocketOut);
        this.Save();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearInput(FormulaSocketIn mySocketIn)
    {
        if (this != mySocketIn.Owner)
        {
            Debug.LogWarning("Can pass only socket that belongs to this Mod");
            return;
        }

        if (null != mySocketIn.Link)
        {
            // remove old connection
            var owner = mySocketIn.Link.Owner;
            owner.m_outputs.Remove(mySocketIn.Link);
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(mySocketIn.Link);
            owner.Save();

            mySocketIn.Link = null;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetSocketIndex(FormulaSocket mySocket)
    {
        if (mySocket.SocketType == FormulaSocketType.In)
        {
            return m_inputs.IndexOf(mySocket as FormulaSocketIn);
        }
        else
        {
            return m_outputs.IndexOf(mySocket as FormulaSocketOut);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ChangeValue<T>(ref T field, T value)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        this.Save();

        Formula?.InvokeChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ChangeValue(ref float field, float value)
    {
        if (Mathf.Approximately(field, value))
        {
            return;
        }

        field = value;
        this.Save();

        Formula?.InvokeChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void DirtySetValue<T>(ref T field, T value)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        UnityEditor.EditorUtility.SetDirty(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void DirtySetValue(ref float field, float value)
    {
        if (Mathf.Approximately(field, value))
        {
            return;
        }

        field = value;
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract float Calculate();


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public abstract string GenerateCode(HashSet<int> vars, StringBuilder builder);
}