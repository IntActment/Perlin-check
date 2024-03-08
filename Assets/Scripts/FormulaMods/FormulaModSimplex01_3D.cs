using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSimplex01_3D : FormulaMod
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
    private float m_offsetZ;

    public float OffsetZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_offsetZ;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_offsetZ, value);
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
    private float m_mulZ = 1;

    public float MulZ
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => m_mulZ;
#if UNITY_EDITOR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => this.ChangeValue(ref m_mulZ, value);
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
        name = "Simplex01 3D";

        await AddInput("X");
        await AddInput("Y");
        await AddInput("Z");
        await AddInput("OffsetX", true);
        await AddInput("OffsetY", true);
        await AddInput("OffsetZ", true);
        await AddInput("MulX", true);
        await AddInput("MulY", true);
        await AddInput("MulZ", true);
    }
#endif

    private static readonly string m_varPrefix = "simplex01_3D";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        return SimplexNoise.Singleton.Multi01(
            m_octaves,
            PickValue(3, m_offsetX) + Inputs[0].CalculateInput() * PickValue(6, m_mulX),
            PickValue(4, m_offsetY) + Inputs[1].CalculateInput() * PickValue(7, m_mulY),
            PickValue(5, m_offsetZ) + Inputs[2].CalculateInput() * PickValue(8, m_mulZ));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = Inputs[1].GenerateCode(vars, builder);
            var val2 = Inputs[2].GenerateCode(vars, builder);

            builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>SimplexNoise</color>.Singleton.<color=#74531f>Multi01</color>(");
            builder.AppendLine($"            {m_octaves},");
            builder.AppendLine($"            {PickCode(3, m_offsetX, vars, builder)} + {val0} * {PickCode(6, m_mulX, vars, builder)},");
            builder.AppendLine($"            {PickCode(4, m_offsetY, vars, builder)} + {val1} * {PickCode(7, m_mulY, vars, builder)},");
            builder.AppendLine($"            {PickCode(5, m_offsetZ, vars, builder)} + {val2} * {PickCode(8, m_mulZ, vars, builder)});");
        }

        return VarName;
    }
}
