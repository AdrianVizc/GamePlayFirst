#ifndef NORMALIZED_SCREEN_UV_INCLUDED
#define NORMALIZED_SCREEN_UV_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float2 GetNormalizedScreenUV(float4 ScreenPosition)
{
#ifdef SHADERGRAPH_PREVIEW
    return float2(0.0, 0.0); // Safe fallback for preview
#else
    return float2(ScreenPosition.x / _ScreenParams.x, ScreenPosition.y / _ScreenParams.y);
#endif
}

#endif // NORMALIZED_SCREEN_UV_INCLUDED
