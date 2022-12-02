struct appdata
{
    float4 vertex : POSITION;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct SurfaceData
{
    half4 BaseColor;
    half4 Normal;
    half Metallic;
    half Smoothness;
    float4 Emission;
};

float3 ad(appdata v)
{
    float3 n = v.normal;
    return n;
}

half4 Diffuse(sampler2D tex, float2 uv, half4 bcol)
{ 
    return half4(tex2D(tex, uv) * bcol);
}

SurfaceData SurfaceProcess (
    sampler2D baseTex, float2 uv_baseTex, half4 baseCol,
    sampler2D mseTex, float2 uv_mseTex,
    sampler2D normalTex, float2 uv_normalTex,
    half2 metalMinMax,
    half2 smoothMinMax,
    float4 emitCol
)
{
    SurfaceData so;
    so.BaseColor = tex2D(baseTex, uv_baseTex) * baseCol; 
    
    //Normal Mapping
    so.Normal = tex2D(normalTex, uv_normalTex);

    // Metallic, Smoothness, Emission
    half3 mse = tex2D(mseTex, uv_mseTex);
    so.Metallic = lerp(metalMinMax.x, metalMinMax.y, mse.r);
    so.Smoothness = lerp(smoothMinMax.x, smoothMinMax.y, mse.g);
    so.Emission = mse.b * emitCol;
    return so;
}



float3 FakeRefraction(appdata v, float3 viewDir, half3 normal, float3 worldPos, half3 baseCol, sampler2D background)
{
    float3 or;
    //or = reflect(normalize(viewDir), normal.rgb);
    //or.b = abs(or.b);
    //or.b = 200 / abs(or.b);
    //or.rg = or.rg * or.b;
    //or.rg = or.rg * (1 / 1024);
    //or.rg = or.rg + (worldPos / 4).rg;
    //float2 uv = (normal + ((worldPos + float3(0, -0.25, 0)) / 4)).rg;
   //or = lerp(baseCol, (tex2D(background, uv).rgb * baseCol), pow(saturate(-normal.b), 4));
    or = ad(v);
    return or;
    //return or;
}