using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSum : FormulaMod
{
    [SerializeField]
    private float m_augend = 0;

    public float Augend
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_augend;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_augend, value);
#endif
    }

    [SerializeField]
    private float m_addend = 0;

    public float Addend
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_addend;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_addend, value);
#endif
    }

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "a + b";

        await AddInput("Augend", true);
        await AddInput("Addend", true);
    }
#endif

    private static readonly string m_varPrefix = "sum";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return PickValue(0, m_augend) + PickValue(1, m_addend);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = PickCode(0, m_augend, vars, builder);
            var val1 = PickCode(1, m_addend, vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = {val0} + {val1};");
        }

        return VarName;
    }
}
