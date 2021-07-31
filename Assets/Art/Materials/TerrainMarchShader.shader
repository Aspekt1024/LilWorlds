Shader "Aspekt/TerrainMarchShader"
{
    Properties
    {
        _WaterHeight ("Water Height", Range(-1.0, .02)) = -0.1
        _SnowHeight ("Snow Height", Range(0.01, 1)) = 0.9
        _GrassMaxHeight ("Grass Max Height", Range(-0.5, 1)) = 0.1
        
        _WaterTex ("Water", 2D) = "blue" {}
        _SandTex ("Sand", 2D) = "yellow" {}
        _GrassTex ("Grass", 2D) = "green" {}
        _MountainTex ("Mountain", 2D) = "brown" {}
        _PeaksTex ("Peaks", 2D) = "white" {}
        
        _WaterNormal ("Water Bump", 2D) = "bump" {}
        _SandNormal ("Sand Bump", 2D) = "bump" {}
        _GrassNormal ("Grass Bump", 2D) = "bump" {}
        _MountainNormal ("Mountain Bump", 2D) = "bump" {}
        _PeaksNormal ("Peaks Bump", 2D) = "bump" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 wpos : TEXCOORD2;
                float3 normal : TEXCOORD3;
                float4 pos : SV_POSITION;
                fixed4 col : COLOR;
                SHADOW_COORDS(1)
            };

            sampler2D _WaterTex;
            sampler2D _SandTex;
            sampler2D _GrassTex;
            sampler2D _MountainTex;
            sampler2D _PeaksTex;
            
            sampler2D _WaterNormal;
            sampler2D _SandNormal;
            sampler2D _GrassNormal;
            sampler2D _MountainNormal;
            sampler2D _PeaksNormal;
            
            float4 _WaterTex_ST;
            float4 _SandTex_ST;
            float4 _GrassTex_ST;
            float4 _MountainTex_ST;
            float4 _PeaksTex_ST;

            half _WaterHeight;
            half _SnowHeight;
            half _GrassMaxHeight;

            float _MinHeight;
            float _MaxHeight;
            float _Size;

            UNITY_DECLARE_TEX2DARRAY(baseTextures);

            float3 triplanar(float3 worldPos, float3 worldNormal, sampler2D tex, float scale)
            {
                float3 scaledWorldPos = worldPos * scale;
                half2 uvX = scaledWorldPos.zy;
                half2 uvY = scaledWorldPos.xz;
                half2 uvZ = scaledWorldPos.xy;

                half3 diffX = tex2D(tex, uvX);
                half3 diffY = tex2D(tex, uvY);
                half3 diffZ = tex2D(tex, uvZ);

                half blendSharpness = 1.0;
                half3 blendWeights = pow(abs(worldNormal), blendSharpness);
                blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);
                
                return diffX * blendWeights.x + diffY * blendWeights.y + diffZ * blendWeights.z;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                float d = saturate(dot(float3(0,1,0), v.normal));
                float dp = pow(d, 100);

                half w = v.vertex.y < _Size * 0.507 ? 1 : 0;
                half p = v.vertex.y > _Size * 0.7f ? saturate((d - 0.5) * 10) : 0;
                half g = v.vertex.y > _Size * 0.5 ? saturate((d - 0.5) * 10) - w - p : 0;
                half m = 1 - g - w - p;
                o.col = fixed4(w, g, m, p);
                
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half shadow = 1.0 - (1.0 - nl) * 0.8;
                o.col.rgb *= shadow * _LightColor0;
                TRANSFER_SHADOW(o)

                o.wpos = v.vertex;
                o.normal = v.normal;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.col;
                col.rgb = 0;

                col.rgb += triplanar(i.wpos, i.normal, _WaterTex, 0.2) * i.col.r;
                col.rgb += triplanar(i.wpos, i.normal, _GrassTex, 0.2) * i.col.g;
                col.rgb += triplanar(i.wpos, i.normal, _MountainTex, 0.2) * i.col.b;
                col.rgb += triplanar(i.wpos, i.normal, _PeaksTex, 1) * i.col.a;

                fixed shadow = SHADOW_ATTENUATION(i);
                shadow = 1 - (1 - shadow) * 0.6;
                col *= shadow;
                
                return col;
            }
            ENDCG
        }
        Pass {
            Tags{"LightMode" = "ShadowCaster"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                V2F_SHADOW_CASTER;
            };

            v2f vert (appdata v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
}
