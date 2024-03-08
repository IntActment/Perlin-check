using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSimplex01_2D : FormulaMod
{
    [SerializeField]
    private float m_offsetX;

    public float OffsetX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_offsetX;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_offsetX, value);
#endif
    }

    [SerializeField]
    private float m_offsetY;

    public float OffsetY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_offsetY;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_offsetY, value);
#endif
    }

    [SerializeField]
    private float m_mulX = 1;

    public float MulX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mulX;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_mulX, value);
#endif
    }

    [SerializeField]
    private float m_mulY = 1;

    public float MulY
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mulY;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_mulY, value);
#endif
    }

    [SerializeField]
    private int m_octaves = 1;

    public int Octaves
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_octaves;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_octaves, value);
#endif
    }

#if UNITY_EDITOR
    protected override async Task Initialize()
    {
        name = "Simplex01 2D";

        await AddInput("X");
        await AddInput("Y");
        await AddInput("OffsetX", true);
        await AddInput("OffsetY", true);
        await AddInput("MulX", true);
        await AddInput("MulY", true);
    }
#endif

    private static readonly string m_varPrefix = "simplex01_2D";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return SimplexNoise.Singleton.MultiNoise01(
            m_octaves,
            PickValue(2, m_offsetX) + Inputs[0].CalculateInput() * PickValue(4, m_mulX),
            PickValue(3, m_offsetY) + Inputs[1].CalculateInput() * PickValue(5, m_mulY));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = Inputs[1].GenerateCode(vars, builder);
            var val2 = PickCode(2, m_offsetX, vars, builder);
            var val3 = PickCode(3, m_offsetY, vars, builder);
            var val4 = PickCode(4, m_mulX, vars, builder);
            var val5 = PickCode(5, m_mulY, vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>SimplexNoise</color>.Singleton.<color=#74531f>MultiNoise01</color>(");
            builder.AppendLine($"            {m_octaves},");
            builder.AppendLine($"            {val2} + {val0} * {val4},");
            builder.AppendLine($"            {val3} + {val1} * {val5});");
        }

        return VarName;
    }
}
