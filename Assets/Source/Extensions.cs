using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEngine;

public static partial class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SetValue<T>(this ScriptableObject o, ref T field, T value)
    {
        if (Equals(field, value))
        {
            return false;
        }

        field = value;
        o.Save();

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Save(this ScriptableObject o)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(o);
        //UnityEditor.AssetDatabase.SaveAssetIfDirty(o);
        //Debug.Log("Saved");
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Bounds CombineWith(this Bounds lhs, Bounds rhs)
    {
        lhs.Encapsulate(rhs);

        return lhs;
    }
}
