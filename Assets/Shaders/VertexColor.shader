Shader "VertexColor"
{
    Properties
    {
        _CutColor ("CutColor", Color) = (1,1,1,1)
        _Cutout ("Cutout", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        #pragma target 3.0

        float4 _CutColor;
        float _Cutout;

        struct Input
        {
            float4 vertColor;
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.vertColor = v.color;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            if (IN.vertColor.r >= _Cutout)
            {
                o.Albedo = _CutColor.rgb;
            }
            else
            {
                o.Albedo = float3(
                    trunc(IN.vertColor.r * 10) / 10.0,
                    trunc(IN.vertColor.g * 10) / 10.0,
                    trunc(IN.vertColor.b * 10) / 10.0);
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}