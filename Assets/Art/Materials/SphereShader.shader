Shader "Aspekt/SphereShader"
{
    Properties
    {
        _WaterHeight ("Water Height", Range(0.01, 0.2)) = 0.05
        _SnowHeight ("Snow Height", Range(0.01, 1)) = 0.55
        _GrassMaxHeight ("Grass Max Height", Range(0.01, 1)) = 0.1
        
        _WaterTex ("Water", 2D) = "blue" {}
        _SandTex ("Sand", 2D) = "yellow" {}
        _GrassTex ("Grass", 2D) = "green" {}
        _MountainTex ("Mountain", 2D) = "brown" {}
        _PeaksTex ("Peaks", 2D) = "white" {}
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
            };

            sampler2D _WaterTex;
            sampler2D _SandTex;
            sampler2D _GrassTex;
            sampler2D _MountainTex;
            sampler2D _PeaksTex;
            
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
                o.uv = TRANSFORM_TEX(v.uv, _GrassTex);
                o.wpos = v.vertex;
                o.normal = v.normal;

                fixed4 terrain = 0;

                float hdir = length(v.vertex);
                float d = saturate(dot(v.vertex, v.normal));
                float dp = pow(d, 10);

                float heightPercent = (length(v.vertex) - _MinHeight) / (_MaxHeight - _MinHeight);
                
                float grassMaxHeight = 0.7;
                float waterHeight = 0.3;
                float peaksHeight = 0.9;

                float w = saturate((waterHeight - heightPercent) * 1000);
                
                float g = saturate((grassMaxHeight - heightPercent) * (1 / grassMaxHeight) * 5);
                terrain.r = w;
                terrain.g = saturate((g - w) * (dp) * 10000);
                terrain.b = 1 - saturate(g + w);
                
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                half shadow = 1.0 - (1.0 - nl) * 0.8;

                o.col = terrain * shadow;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,1);
                col.rgb += triplanar(i.wpos, i.normal, _WaterTex, 30) * i.col.r;
                col.rgb += triplanar(i.wpos, i.normal, _GrassTex, 30) * i.col.g;
                col.rgb += triplanar(i.wpos, i.normal, _MountainTex, 0.2) * i.col.b;
                col.rgb += triplanar(i.wpos, i.normal, _PeaksTex, 1) * i.col.a;
                
                return col;
            }
            ENDCG
        }
    }
}
