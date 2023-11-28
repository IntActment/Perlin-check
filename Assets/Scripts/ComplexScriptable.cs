using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ComplexScriptable : ScriptableObject
{

#if UNITY_EDITOR

    private void Awake()
    {
        Init();
    }

    private void OnValidate()
    {
        Init();
    }

    private void Reset()
    {
        Init();
    }

    private void OnDestroy()
    {
        UnityEditor.EditorApplication.update -= DelayedInit;
    }

    protected abstract void OnLateInit();

    private void Init()
    {
        // If this asset already exists initialize immediately
        if (UnityEditor.AssetDatabase.Contains(this))
        {
            DelayedInit();
        }

        // otherwise attach a callback to the editor update to re-check repeatedly until it exists
        // this means it is currently being created an the name has not been confirmed yet
        else
        {
            UnityEditor.EditorApplication.update -= DelayedInit;
            UnityEditor.EditorApplication.update += DelayedInit;
        }
    }

    private void DelayedInit()
    {
        // if this asset dos still not exist do nothing
        // this means it is currently being created and the name not confirmed yet
        if (false == UnityEditor.AssetDatabase.Contains(this))
        {
            return;
        }

        // as soon as the asset exists remove the callback as we don't need it anymore
        UnityEditor.EditorApplication.update -= DelayedInit;

        // first try to find existing child within all assets contained in this asset
        //var assets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));

        OnLateInit();
    }
#endif
}
