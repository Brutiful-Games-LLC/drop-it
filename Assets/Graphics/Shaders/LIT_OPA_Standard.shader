Shader "Custom/LIT_OPA_Standard"
{
    Properties
    {
        [NoScaleOffset] _NormalTex("Normal Texture", 2D) = "blue" {}
        [NoScaleOffset] _BaseTex("Base Texture", 2D) = "white" {}
        [NoScaleOffset] _MSETex("MSE Texture", 2D) = "white" {}
        _BaseCol ("Base Color", Color) = (1,1,1,1)
        [ShowAsVector2] _MetalMinMax ("Metallic Min Max", Vector) = (0,1,0,0)
        [ShowAsVector2] _SmoothMinMax("Smoothness Min Max", Vector) = (0,1,0,0)
        [HDR] _EmitCol("Emissive Color", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "Functions.hlsl"

        sampler2D _NormalTex;
        sampler2D _BaseTex;
        half4 _BaseCol;
        sampler2D _MSETex;
        half2 _MetalMinMax;
        half2 _SmoothMinMax;
        half4 _EmitCol;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;
            float3 worldRefl; INTERNAL_DATA

            float2 uv_NormalTex;
            float2 uv_BaseTex;
            float2 uv_MSETex;
            appdata v;
        };


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            SurfaceData so = SurfaceProcess(
                _BaseTex, IN.uv_BaseTex, _BaseCol,
                _MSETex, IN.uv_MSETex,
                _NormalTex, IN.uv_MSETex,
                _MetalMinMax,
                _SmoothMinMax,
                _EmitCol
            );
            // Albedo comes from a texture tinted by color
            o.Albedo = so.BaseColor.rgb;
            
            //Normal Mapping
            o.Normal = UnpackNormal(so.Normal);
            

            // Metallic, Smoothness, Emission
            o.Metallic = so.Metallic;
            o.Smoothness = so.Smoothness;
            o.Emission = so.Emission;

            o.Alpha = so.BaseColor.a;

            /*
            // Albedo comes from a texture tinted by color
            half4 c = tex2D(_BaseTex, IN.uv_BaseTex) * _BaseCol;
            half3 mse = tex2D(_MSETex, IN.uv_MSETex);
            o.Albedo = c.rgb;

            //Normal Mapping
            o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));

            // Metallic, Smoothness, Emission
            o.Metallic = lerp(_MetalMinMax.x, _MetalMinMax.y, mse.r);
            o.Smoothness = lerp(_SmoothMinMax.x, _SmoothMinMax.y, mse.g);
            o.Emission = mse.b * _EmitCol;

            o.Alpha = c.a;
            */
        }
        ENDCG
    }
    FallBack "Diffuse"
}
