#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

#if defined(SHADERGRAPH_PREVIEW)
// Dummy fallback for Shader Graph Preview
float2 GetNormalizedScreenUV_float(float ScreenPosition)
{
    return float2(0.0, 0.0); // Just return something valid
}
#else
// Actual function used in runtime
float2 GetNormalizedScreenUV(float4 ScreenPosition)
{
    return float2(ScreenPosition.x / _ScreenParams.x, ScreenPosition.y / _ScreenParams.y);
}
#endif
