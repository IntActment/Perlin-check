using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

using UnityEngine;

[CreateAssetMenu(fileName = "Complex formula", menuName = "Create complex formula", order = 0)]
public class ComplexFormula : ComplexScriptable
{
#if UNITY_EDITOR
    public delegate void FormulaChanged(ComplexFormula formula);

    public static event FormulaChanged OnFormulaChanged;
#endif

    [SerializeField]
    private List<FormulaMod> m_modifiers;

    public IReadOnlyList<FormulaMod> Modifiers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_modifiers;
    }

#if UNITY_EDITOR
    [SerializeField]
    private float m_zoom = 1;

    public float Zoom
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_zoom;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => DirtySetValue(ref m_zoom, value);
    }

    [SerializeField]
    private Vector2 m_screenOffset;

    public Vector2 ScreenOffset
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_screenOffset;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => DirtySetValue(ref m_screenOffset, value);
    }
#endif

    [SerializeField]
    private FormulaModInput m_inputX;

    public FormulaModInput InputX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_inputX;
    }

    [SerializeField]
    private FormulaModInput m_inputY;

    public FormulaModInput InputY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_inputY;
    }

    [SerializeField]
    private FormulaModOutput m_output;

    public FormulaModOutput Output
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_output;
    }

#if UNITY_EDITOR
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
    public T CreateMod<T>(string name, Vector2 pos) where T : FormulaMod
    {
        var ret = CreateInstance<T>();
        ret.name = name;
        ret.Position = pos;
        ret.Formula = this;
        ret.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;

        m_modifiers.Add(ret);
        UnityEditor.AssetDatabase.AddObjectToAsset(ret, this);
        this.Save();

        return ret;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Delete(FormulaMod mod)
    {
        if (m_modifiers.Remove(mod))
        {
            while (mod.Outputs.Count > 0)
            {
                mod.RemoveOutput(mod.Outputs[0]);
            }

            foreach (var input in mod.Inputs)
            {
                mod.ClearInput(input);
            }

            UnityEditor.AssetDatabase.RemoveObjectFromAsset(mod);
            this.Save();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BringToFront(int index)
    {
        var mod = m_modifiers[index];

        m_modifiers.RemoveAt(index);
        m_modifiers.Add(mod);

        this.Save();
    }

    private TextAsset m_monoScript;


    private void CreateScript()
    {
        var path = UnityEditor.AssetDatabase.GetAssetPath(this);
        Debug.Log(path);

        var content = @"
     using System;
 
     public class CustomClass {
 
     }
 
     ";

        var fullPath = path + "code.cs";

        File.WriteAllText(fullPath, content);
        // Need to import the newly created file, otherwise it won't appear in the
        // editor.
        UnityEditor.AssetDatabase.ImportAsset(fullPath);

        m_monoScript = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(fullPath);
        m_monoScript.name = $"{name} code";
        UnityEditor.AssetDatabase.AddObjectToAsset(m_monoScript, this);
    }

    protected override void OnLateInit()
    {
        if (null == Modifiers)
        {
            m_modifiers = new List<FormulaMod>();

            m_inputX = CreateMod<FormulaModInput>("[In] X", new Vector2(20, 40));
            m_inputY = CreateMod<FormulaModInput>("[In] Y", new Vector2(20, 100));
            m_output = CreateMod<FormulaModOutput>("[Out]", new Vector2(300, 60));
            CreateScript();
            this.Save();
        }

        InvokeChanged();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InvokeChanged()
    {
        OnFormulaChanged?.Invoke(this);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Calculate(float x, float y)
    {
        InputX.Value = x;
        InputY.Value = y;

        return Output.Calculate();
    }
}
