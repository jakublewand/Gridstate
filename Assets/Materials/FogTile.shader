Shader "Custom/FogTile"
{
    Properties
    {
        _Color ("Fog Color", Color) = (0.85, 0.88, 0.92, 0.6)
        _Density ("Density", Range(0, 1)) = 0.6
        _Speed ("Drift Speed", Range(0, 2)) = 0.3
        _NoiseScale ("Noise Scale", Range(0.5, 10)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float _Density;
            float _Speed;
            float _NoiseScale;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 localPos : TEXCOORD1;
            };

            // 3D hash
            float hash3(float3 p)
            {
                p = frac(p * float3(0.1031, 0.1030, 0.0973));
                p += dot(p, p.yxz + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            // 3D value noise
            float noise3(float3 p)
            {
                float3 i = floor(p);
                float3 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float n000 = hash3(i);
                float n100 = hash3(i + float3(1, 0, 0));
                float n010 = hash3(i + float3(0, 1, 0));
                float n110 = hash3(i + float3(1, 1, 0));
                float n001 = hash3(i + float3(0, 0, 1));
                float n101 = hash3(i + float3(1, 0, 1));
                float n011 = hash3(i + float3(0, 1, 1));
                float n111 = hash3(i + float3(1, 1, 1));

                float nx00 = lerp(n000, n100, f.x);
                float nx10 = lerp(n010, n110, f.x);
                float nx01 = lerp(n001, n101, f.x);
                float nx11 = lerp(n011, n111, f.x);

                float nxy0 = lerp(nx00, nx10, f.y);
                float nxy1 = lerp(nx01, nx11, f.y);

                return lerp(nxy0, nxy1, f.z);
            }

            // 3D fbm
            float fbm3(float3 p)
            {
                float val = 0.0;
                float amp = 0.5;
                for (int i = 0; i < 4; i++)
                {
                    val += amp * noise3(p);
                    p *= 2.0;
                    amp *= 0.5;
                }
                return val;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.localPos = v.vertex.xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                // World-space 3D coords for seamless tiling
                float3 p = i.worldPos * _NoiseScale;
                float t = _Time.y * _Speed;

                // Two drifting 3D noise layers for swirl effect
                float n1 = fbm3(p + float3(t * 0.6, t * 0.1, t * 0.3));
                float n2 = fbm3(p * 1.3 + float3(-t * 0.4, t * 0.15, t * 0.5));
                float n = (n1 + n2) * 0.5;

                // Vertical fade: strongest at base, fades at top
                // localPos.y ranges from -0.5 to 0.5 on a unit cube
                float heightFade = saturate(1.0 - (i.localPos.y + 0.5));

                // Edge fade on horizontal borders for softness
                float2 edgeDist = saturate(abs(i.localPos.xz) * 4.0 - 1.0);
                float edgeFade = 1.0 - max(edgeDist.x, edgeDist.y);
                edgeFade = saturate(edgeFade);

                // Edge proximity: 1 at center, 0 at border
                float2 edge = smoothstep(0.0, 0.25, 0.5 - abs(i.localPos.xz));
                float edgeFactor = edge.x * edge.y;

                // Raise the noise threshold near edges — only dense peaks survive
                float threshold = lerp(0.8, 0.0, edgeFactor);
                float cloud = saturate((n - threshold) / (1.0 - threshold));

                float alpha = cloud * heightFade * _Density * _Color.a;

                return float4(_Color.rgb, alpha);
            }
            ENDCG
        }
    }

    Fallback Off
}
