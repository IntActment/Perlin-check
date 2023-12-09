using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public abstract class ComplexScriptable : ScriptableObject
{
#if UNITY_EDITOR

    private async void Awake()
    {
        await Init();
    }

    private async void OnValidate()
    {
        await Init();
    }

    private async void Reset()
    {
        await Init();
    }

    private void OnDestroy()
    {

    }

    private bool m_isInitializing = false;

    [SerializeField]
    private bool m_isInit = false;

    public bool IsInitialized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_isInit;
    }

    private async Task Init()
    {
        if (true == m_isInitializing)
        {
            return;
        }

        if (true == m_isInit)
        {
            return;
        }
        else
        {
            m_isInitializing = true;
        }

        while ((true == EditorApplication.isUpdating) || (false == AssetDatabase.Contains(this)))
        {
            await Task.Delay(1);
        }

        await OnLateInit();

        m_isInit = true;
    }

    public async Task WaitInit()
    {
        while (false == m_isInit)
        {
            await Task.Delay(1);
        }
    }

    protected virtual async Task OnLateInit()
    {
        await Task.CompletedTask;
    }

    protected void AddSubAsset(Object subAsset)
    {
        AssetDatabase.AddObjectToAsset(subAsset, this);
    }
#endif
}
