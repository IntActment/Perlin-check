using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public class FormulaModSimplex01 : FormulaMod
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
        name = "Simplex01";

        await AddInput("X");
        await AddInput("Y");
        await AddInput("Z", true);
    }

    protected override async Task OnLateAwake()
    {
        if (m_inputs.Count < 4)
        {
            await AddInput("OffsetX", true);
        }

        if (m_inputs.Count < 5)
        {
            await AddInput("OffsetY", true);
        }

        if (m_inputs.Count < 6)
        {
            await AddInput("OffsetZ", true);
        }

        if (m_inputs.Count < 7)
        {
            await AddInput("MulX", true);
        }

        if (m_inputs.Count < 8)
        {
            await AddInput("MulY", true);
        }

        if (m_inputs.Count < 9)
        {
            await AddInput("MulZ", true);
        }
    }
#endif

    private static readonly string m_varPrefix = "simplex01";
    public override string VarPrefix => m_varPrefix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override float Calculate()
    {
        if (Inputs[2].Link == null)
        {
            return SimplexNoise.Singleton.MultiNoise01(
                m_octaves,
                m_offsetX + Inputs[0].CalculateInput() * m_mulX,
                m_offsetY + Inputs[1].CalculateInput() * m_mulY);

        }
        else
        {
            return SimplexNoise.Singleton.Multi01(
                m_octaves,
                m_offsetX + Inputs[0].CalculateInput() * m_mulX,
                m_offsetY + Inputs[1].CalculateInput() * m_mulY,
                m_offsetZ + Inputs[2].CalculateInput() * m_mulZ);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GenerateCode(HashSet<int> vars, StringBuilder builder)
    {
        if (false == vars.Contains(VarIndex))
        {
            vars.Add(VarIndex);

            var val0 = Inputs[0].GenerateCode(vars, builder);
            var val1 = Inputs[1].GenerateCode(vars, builder);

            if (Inputs[2].Link == null)
            {
                builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>SimplexNoise</color>.Singleton.<color=#74531f>MultiNoise01</color>(");
                builder.AppendLine($"            {m_octaves},");
                builder.AppendLine($"            {m_offsetX}f + {val0} * {m_mulX}f,");
                builder.AppendLine($"            {m_offsetY}f + {val1} * {m_mulY}f);");
            }
            else
            {
                var val2 = Inputs[2].GenerateCode(vars, builder);

                builder.AppendLine($"        <color=blue>float</color> {VarName} = <color=#2b91af>SimplexNoise</color>.Singleton.<color=#74531f>Multi01</color>(");
                builder.AppendLine($"            {m_octaves},");
                builder.AppendLine($"            {m_offsetX}f + {val0} * {m_mulX}f,");
                builder.AppendLine($"            {m_offsetY}f + {val1} * {m_mulY}f,");
                builder.AppendLine($"            {m_offsetZ}f + {val2} * {m_mulZ}f);");
            }
        }

        return VarName;
    }
}
