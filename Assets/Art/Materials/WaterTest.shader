Shader "Aspekt/WaterTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Bump ("Bump", 2D) = "bump" {}
		_ScaleUV ("Scale", Range(0.001,10)) = 1
    	
    	_EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
    	_DepthFactor("Depth Factor", Range(0.0, 1.5)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        GrabPass{}
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 uvgrab : TEXCOORD1;
				float2 uvbump : TEXCOORD2;
            	float4 screenPos : TEXCOORD3;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
            };

            sampler2D _CameraDepthTexture;

			sampler2D _GrabTexture;
			float4 _GrabTexture_TexelSize;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Bump;
            float4 _Bump_ST;
			float _ScaleUV;
            float _DepthFactor;
            fixed4 _EdgeColor;

            v2f vert (appdata v)
            {
				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif
                
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
            	o.screenPos = ComputeScreenPos(o.pos);
                o.uvgrab.xy = (float2(o.pos.x, o.pos.y * scale) + o.pos.w) * 0.5;
				o.uvgrab.zw = o.pos.zw;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				half2 bump = UnpackNormal(tex2D(_Bump, i.uvbump / _ScaleUV)).rg;
				float2 offset = bump * _ScaleUV * _GrabTexture_TexelSize.xy;
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
				
				fixed4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
				fixed4 waterTint = tex2D(_MainTex, i.uv / _ScaleUV);
                col *= waterTint;

            	float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
            	float depth = LinearEyeDepth(depthSample).r;

            	float d = saturate(_DepthFactor * depth - i.screenPos.w);
            	float4 foamLine = 1 - d;

            	col = (d * 0.6) * waterTint + (d * 0.4) * col + foamLine * _EdgeColor;
            	
                return col;
            }
            ENDCG
        }
    }
}
