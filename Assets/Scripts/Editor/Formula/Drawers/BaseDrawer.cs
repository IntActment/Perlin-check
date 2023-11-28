using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine;

public abstract class BaseDrawer : ModDrawer<FormulaMod>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BaseDrawer(FormulaMod mod)
        : base(mod) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override Vector2 GetBodySize()
    {
        return default;
    }

    public delegate BaseDrawer GetFormulaDrawerDelegate(FormulaMod mod);

    private static Dictionary<System.Type, GetFormulaDrawerDelegate> m_instances = new Dictionary<System.Type, GetFormulaDrawerDelegate>();

    protected static void Register(System.Type type, GetFormulaDrawerDelegate factory)
    {
        m_instances.Add(type, factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BaseDrawer GetInstance(FormulaMod mod)
    {
        return m_instances[mod.GetType()](mod);
    }

    static BaseDrawer()
    {
        var datasetDerrivedTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(BaseDrawer).IsAssignableFrom(t) && t != typeof(BaseDrawer));

        foreach (var type in datasetDerrivedTypes)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
    }
}
