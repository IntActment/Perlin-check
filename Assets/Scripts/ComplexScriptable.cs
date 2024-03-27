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
        await Load();
    }

    private async void OnValidate()
    {
        await Load();
    }

    private async void Reset()
    {
        await Load();
    }

    private void OnDestroy()
    {

    }

    private bool m_isLoading = false;

    public bool IsLoading
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_isLoading;
    }

    [SerializeField]
    private bool m_isInit = false;

    public bool IsInitialized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_isInit;
    }

    private async Task Load()
    {
        m_isLoading = true;

        if (false == m_isInit)
        {
            while ((true == EditorApplication.isUpdating) || (false == AssetDatabase.Contains(this)))
            {
                await Task.Delay(1);
            }

            await OnLateInit();

            m_isInit = true;
        }

        await OnLateAwake();

        m_isLoading = false;
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

    protected virtual async Task OnLateAwake()
    {
        await Task.CompletedTask;
    }

    protected void AddSubAsset(Object subAsset)
    {
        AssetDatabase.AddObjectToAsset(subAsset, this);
    }
#endif
}
