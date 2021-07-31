Shader "Custom/WaterTest3"
{
    // reference https://www.youtube.com/watch?v=vTMEdHcKgM4&t=1003s&ab_channel=SebastianLague
    Properties
    {
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        
        _ShallowColor("Shallow Color", Color) = (1,1,1,1)
        _DeepColor("Deep Color", Color) = (0,0,0,1) 
        _DepthFactor("Depth Factor", Range(0.0, 5.0)) = 1.0
        
        _FresnelPower ("Fresnel Power", Range(0.0, 5.0)) = 1.0
        _ShoreFadeStrength ("Shore Fade Strength", Range(0.0, 50.0)) = 1.0
        
        _Alpha ("Alpha", Range(0.0, 1.0)) = 0.4
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_NormalMap;
            float4 screenPos;
            float3 viewDir;
            float3 worldNormal; INTERNAL_DATA
        };

        half _Smoothness;
        half _Metallic;
        
        fixed4 _ShallowColor;
        fixed4 _DeepColor;
        float _DepthFactor;
        
        float _FresnelPower;
        float _ShoreFadeStrength;

        float _Alpha;
        
        sampler2D _CameraDepthTexture;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float nonLinearDepth = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, IN.screenPos);
            float distToTerrain = LinearEyeDepth(nonLinearDepth);
            float distToWater = IN.screenPos.w;
            float waterViewDepth = distToTerrain - distToWater;

            fixed4 c;
            
            c.rgb = lerp(_ShallowColor, _DeepColor, 1 - exp(-waterViewDepth * _DepthFactor));

            // To add transparency when viewed at a steeper angle
            float fresnel = 1 - min(0.2, pow(saturate(dot(-IN.viewDir, IN.worldNormal)), -_FresnelPower));

            // To be more transparent in shallow regions
            float shoreFade = 1 - exp(-waterViewDepth * _ShoreFadeStrength);
            
            float waterAlpha = fresnel * shoreFade; 
            c.a = waterAlpha;

            float t = 0.5 * (0.5 + _Time.x) * 0.2;
            o.Normal = UnpackNormal(tex2D (_NormalMap, IN.uv_NormalMap * t));
            
            o.Albedo.rgb = c.rgb;
            o.Smoothness = _Smoothness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
