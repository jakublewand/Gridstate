Shader "Skybox/Cubemap Blend"
{
    Properties
    {
        _Blend ("Blend", Range(0, 1)) = 0

        [Header(Sky A)]
        [NoScaleOffset] _CubeA ("Cubemap A", Cube) = "" {}
        _ExposureA ("Exposure A", Range(0, 8)) = 1
        _TintA ("Tint A", Color) = (0.5, 0.5, 0.5, 1)

        [Header(Sky B)]
        [NoScaleOffset] _CubeB ("Cubemap B", Cube) = "" {}
        _ExposureB ("Exposure B", Range(0, 8)) = 1
        _TintB ("Tint B", Color) = (0.5, 0.5, 0.5, 1)

        [Header(Rotation)]
        _RotationA ("Rotation A", Range(0, 360)) = 0
        _RotationB ("Rotation B", Range(0, 360)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            samplerCUBE _CubeA;
            samplerCUBE _CubeB;
            float _Blend;
            float _ExposureA;
            float _ExposureB;
            float4 _TintA;
            float4 _TintB;
            float _RotationA;
            float _RotationB;

            float3 RotateAroundY(float3 dir, float degrees)
            {
                float rad = degrees * UNITY_PI / 180.0;
                float s = sin(rad);
                float c = cos(rad);
                return float3(dir.x * c - dir.z * s, dir.y, dir.x * s + dir.z * c);
            }

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 texcoord : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.texcoord);

                float4 colA = texCUBE(_CubeA, RotateAroundY(dir, _RotationA)) * _ExposureA * _TintA * 2.0;
                float4 colB = texCUBE(_CubeB, RotateAroundY(dir, _RotationB)) * _ExposureB * _TintB * 2.0;

                return lerp(colA, colB, _Blend);
            }
            ENDCG
        }
    }

    Fallback "Skybox/Cubemap"
}
