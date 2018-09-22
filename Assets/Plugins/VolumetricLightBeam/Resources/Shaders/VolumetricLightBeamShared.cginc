// The following comment prevents Unity from auto upgrading the shader. Please keep it to keep backward compatibility
// UNITY_SHADER_NO_UPGRADE

#ifndef _VOLUMETRIC_LIGHT_BEAM_SHARED_INCLUDED_
#define _VOLUMETRIC_LIGHT_BEAM_SHARED_INCLUDED_

#include "UnityCG.cginc"

/// ****************************************
/// DEBUG
/// ****************************************
//#define DEBUG_SHOW_DEPTH 1
//#define DEBUG_SHOW_NOISE3D 1
//#define DEBUG_BLEND_INSIDE_OUTSIDE 1

#if DEBUG_SHOW_DEPTH && !VLB_DEPTH_BLEND
#define VLB_DEPTH_BLEND 1
#endif

#if DEBUG_SHOW_NOISE3D && !VLB_NOISE_3D
#define VLB_NOISE_3D 1
#endif
/// ****************************************

/// ****************************************
/// OPTIM
/// ****************************************
/// compute most of the intensity in VS => huge perf improvements
#define OPTIM_VS 1 

/// when OPTIM_VS is enabled, also compute fresnel in VS => better perf,
/// but require too much tessellation for the same quality
//#define OPTIM_VS_FRESNEL_VS 1
/// ****************************************

#include "ShaderUtils.cginc"

struct v2f
{
    float4 posClipSpace : SV_POSITION;
    float4 posObjectSpace : TEXCOORD0;
    float4 posWorldSpace : TEXCOORD1;
    UNITY_FOG_COORDS(2)
    float4 posViewSpace_extraData : TEXCOORD3;

#if OPTIM_VS
    half4 color : TEXCOORD4;
#endif

#if VLB_NOISE_3D || OPTIM_VS
    float4 uvwNoise_intensity : TEXCOORD5;
#endif

#if VLB_DEPTH_BLEND
    float4 projPos : TEXCOORD6;
#endif

#if VLB_PASS_OUTSIDEBEAM_FROM_VS_TO_FS
    half outsideBeam : TEXCOORD7;
#endif
};


#if VLB_COLOR_GRADIENT_MATRIX_HIGH || VLB_COLOR_GRADIENT_MATRIX_LOW
uniform float4x4 _ColorGradientMatrix;
#elif VLB_COLOR_GRADIENT_ARRAY
#define kColorGradientArraySize 8
uniform float4 _ColorGradientArray[kColorGradientArraySize];
#else
uniform half4 _ColorFlat;
#endif
uniform half _AlphaInside;
uniform half _AlphaOutside;
uniform float2 _ConeSlopeCosSin; // between -1 and +1
uniform float2 _ConeRadius; // x = start radius ; y = end radius
uniform float _ConeApexOffsetZ; // > 0
uniform float _AttenuationLerpLinearQuad;
uniform float _DistanceFadeStart;
uniform float _DistanceFadeEnd;
uniform float _DistanceCamClipping;
uniform float _FresnelPow; // must be != 0 to avoid infinite fresnel
uniform float _GlareFrontal;
uniform float _GlareBehind;
uniform float _DrawCap;
uniform float4 _CameraParams; // xyz: object space forward vector ; w: cameraIsInsideBeamFactor (-1 : +1)
uniform float3 _CameraPosObjectSpace;  // Camera Position in Object Space

#if VLB_CLIPPING_PLANE
uniform float4 _ClippingPlaneWS;
#endif

#if VLB_DEPTH_BLEND
uniform float _DepthBlendDistance;
#endif


inline float ComputeBoostFactor(float pixDistFromSource, float outsideBeam, float isCap)
{
    pixDistFromSource = max(pixDistFromSource, 0.001); // prevent 1st segment from being boosted when boostFactor is 0
    float insideBoostDistance = _GlareFrontal * _DistanceFadeEnd;
    float boostFactor = 1 - smoothstep(0, 0 + insideBoostDistance + 0.001, pixDistFromSource); // 0 = no boost ; 1 = max boost
    boostFactor = lerp(boostFactor, 0, outsideBeam); // no boost for outside pass

    float cameraIsInsideBeamFactor = saturate(_CameraParams.w); // _CameraParams.w is (-1 ; 1) 
    boostFactor = cameraIsInsideBeamFactor * boostFactor; // no boost for outside pass

    boostFactor = lerp(boostFactor, 1, isCap); // cap is always at max boost
    return boostFactor;
}

// boostFactor is normalized
float ComputeFresnel(float3 posObjectSpace, float3 vecCamToPixOSN, float outsideBeam, float boostFactor)
{
    // Compute normal
    float2 cosSinFlat = normalize(posObjectSpace.xy);
    float3 normalObjectSpace = (float3(cosSinFlat.x * _ConeSlopeCosSin.x, cosSinFlat.y * _ConeSlopeCosSin.x, -_ConeSlopeCosSin.y));
    normalObjectSpace *= (outsideBeam * 2 - 1); // = outsideBeam ? 1 : -1;
    
    // real fresnel factor
    float fresnelReal = dot(normalObjectSpace, -vecCamToPixOSN);

    // compute a fresnel factor to support long beams by projecting the viewDir vector
    // on the virtual plane formed by the normal and tangent
    float3 tangentPlaneNormal = normalize(posObjectSpace.xyz + float3(0, 0, _ConeApexOffsetZ));
    float distToPlane = dot(-vecCamToPixOSN, tangentPlaneNormal);
    float3 vec2D = normalize(-vecCamToPixOSN - distToPlane * tangentPlaneNormal);
    float fresnelProjOnTangentPlane = dot(normalObjectSpace, vec2D);

    // blend between the 2 fresnels
    float vecCamToPixDotZ = vecCamToPixOSN.z;
    float factorNearAxisZ = abs(vecCamToPixDotZ); // factorNearAxisZ is normalized

    float fresnel = lerp(fresnelProjOnTangentPlane, fresnelReal, factorNearAxisZ);

    
    float fresnelPow = _FresnelPow;

    // Lerp the fresnel pow to the glare factor according to how far we are from the axis Z
    const float kMaxGlarePow = 1.5;
    float glareFactor = kMaxGlarePow * (1 - lerp(_GlareFrontal, _GlareBehind, outsideBeam));
    fresnelPow = lerp(fresnelPow, min(fresnelPow, glareFactor), factorNearAxisZ);
    
    // Pow the fresnel
    fresnel = smoothstep(0, 1, fresnel);
    fresnel = (1 - step(0, -fresnel)) * // fix edges artefacts on android ES2
              (pow(fresnel, fresnelPow));

    // Boost distance inside
    float boostFresnel = lerp(fresnel, 1 + 0.001, boostFactor);
    fresnel = lerp(boostFresnel, fresnel, outsideBeam); // no boosted fresnel if outside

    // We do not have to treat cap a special way, since boostFactor is already set to 1 for cap via ComputeBoostFactor
    
    return fresnel;
}


inline float ComputeFadeWithCamera(float3 posViewSpace, float enabled)
{
    float camFadeDistStart = _ProjectionParams.y; // cam near place
    float camFadeDistEnd = camFadeDistStart + _DistanceCamClipping;
    float distCamToPixWS = abs(posViewSpace.z); // only check Z axis (instead of length(posViewSpace.xyz)) to have smoother transition with near plane (which is not curved)
    float fadeWhenTooClose = smoothstep(0, 1, invLerpClamped(camFadeDistStart, camFadeDistEnd, distCamToPixWS));

    return lerp(1, fadeWhenTooClose, enabled);
}

half4 ComputeColor(float pixDistFromSource, float outsideBeam)
{
#if VLB_COLOR_GRADIENT_MATRIX_HIGH || VLB_COLOR_GRADIENT_MATRIX_LOW
    float distFromSourceNormalized = invLerpClamped(0, _DistanceFadeEnd, pixDistFromSource);
    half4 color = DecodeGradient(distFromSourceNormalized, _ColorGradientMatrix);
#elif VLB_COLOR_GRADIENT_ARRAY
    float distFromSourceNormalized = invLerpClamped(0, _DistanceFadeEnd, pixDistFromSource);
    half4 color = DecodeGradient(distFromSourceNormalized, _ColorGradientArray);
#else
    half4 color = _ColorFlat;
#endif

    half alpha = lerp(_AlphaInside, _AlphaOutside, outsideBeam);
#if ALPHA_AS_BLACK
    color.rgb *= color.a;
    color.rgb *= alpha;
#else
    color.a *= alpha;
#endif

    return color;
}

inline float ComputeInOutBlending(float vecCamToPixDotZ, float outsideBeam)
{
    // smooth blend between inside and outside geometry depending of View Direction
    const float kFaceLightSmoothingLimit = 1;
    float factorFaceLightSourcePerPixN = saturate(smoothstep(kFaceLightSmoothingLimit, -kFaceLightSmoothingLimit, vecCamToPixDotZ)); // smoother transition

    return lerp(factorFaceLightSourcePerPixN, 1 - factorFaceLightSourcePerPixN, outsideBeam);
}


v2f vertShared(appdata_base v, float outsideBeam)
{
    v2f o;
#if VLB_PASS_OUTSIDEBEAM_FROM_VS_TO_FS
    o.outsideBeam = outsideBeam;
#endif
    o.uvwNoise_intensity = 1;

    // compute the proper cone shape, so the whole beam fits into a 2x2x1 box
    // The model matrix (computed via the localScale from BeamGeometry.)
    float4 vertexOS = v.vertex;

    vertexOS.z *= vertexOS.z; // make segment tessellation denser near the source, since beam is usually more visible at start

    float maxRadius = max(_ConeRadius.x, _ConeRadius.y);
    float normalizedRadiusStart = _ConeRadius.x / maxRadius;
    float normalizedRadiusEnd = _ConeRadius.y / maxRadius;
    vertexOS.xy *= lerp(normalizedRadiusStart, normalizedRadiusEnd, vertexOS.z);

    o.posClipSpace = UnityObjectToClipPos(vertexOS);
    o.posWorldSpace = mul(matObjectToWorld, vertexOS);

    {
        // apply the same scaling than we do through the localScale in BeamGeometry.ComputeLocalMatrix
        // to get the proper transformed vertex position
        float4 posObjectSpace = vertexOS;
        posObjectSpace.xy *= maxRadius;
        posObjectSpace.z *= _DistanceFadeEnd;
        o.posObjectSpace = posObjectSpace;
    }

#if VLB_DEPTH_BLEND
    o.projPos = DepthFade_VS_ComputeProjPos(vertexOS, o.posClipSpace);
#endif

    float isCap = v.texcoord.x;

#if VLB_NOISE_3D
	o.uvwNoise_intensity.rgb = Noise3D_GetUVW(o.posWorldSpace.xyz);
#endif

#if OPTIM_VS
    // Treat Cap a special way: cap is only visible from inside
    float intensity = 1 - outsideBeam * isCap; // AKA if (outsideBeam == 1 && isCap == 1) intensity = 0

    float pixDistFromSource = length(o.posObjectSpace.z);
    half cameraIsOrtho = unity_OrthoParams.w; // w is 1.0 when camera is orthographic, 0.0 when perspective

    intensity *= ComputeAttenuation(pixDistFromSource, _DistanceFadeStart, _DistanceFadeEnd, _AttenuationLerpLinearQuad);
    float boostFactor = ComputeBoostFactor(pixDistFromSource, outsideBeam, isCap);

    // Vector Camera to current Pixel, in object space and normalized
    float3 vecCamToPixOSN = normalize(o.posObjectSpace.xyz - _CameraPosObjectSpace);

    // Deal with ortho camera:
    // With ortho camera, we don't want to change the fresnel according to camera position.
    // So instead of computing the proper vector "Camera to Pixel", we take account of the "Camera Forward" vector (which is not dependant on the pixel position)
    float3 vecCamForwardOSN = _CameraParams.xyz;
    vecCamToPixOSN = lerp(vecCamToPixOSN, vecCamForwardOSN, cameraIsOrtho);

    float vecCamToPixDotZ = dot(vecCamToPixOSN, float3(0, 0, 1));

#if OPTIM_VS_FRESNEL_VS
    // Pass data needed to compute fresnel in fragment shader
    // Computing fresnel on vertex shader give imprecise results
    intensity *= ComputeFresnel(vertexOS, vecCamToPixOSN, outsideBeam, boostFactor);
#endif

    // smooth blend between inside and outside geometry depending of View Direction
    intensity *= ComputeInOutBlending(vecCamToPixDotZ, outsideBeam);

    // no intensity for cap if _DrawCap = 0
    intensity *= step(isCap, _DrawCap);

    o.uvwNoise_intensity.a = intensity;

    o.color = ComputeColor(pixDistFromSource, outsideBeam);

    float extraData = boostFactor;
#else
    float extraData = isCap;
#endif

    float3 posViewSpace = UnityObjectToViewPos(vertexOS);
    o.posViewSpace_extraData = float4(posViewSpace, extraData);

    UNITY_TRANSFER_FOG(o, o.posClipSpace);
    return o;
}

half4 fragShared(v2f i, float outsideBeam)
{
#if OPTIM_VS
    float intensity = i.uvwNoise_intensity.a;
#else
    float intensity = 1;
#endif

#if VLB_CLIPPING_PLANE
    float distToClipPlane = DistanceToPlane(i.posWorldSpace.xyz, _ClippingPlaneWS.xyz, _ClippingPlaneWS.w);
    clip(distToClipPlane);
    intensity *= smoothstep(0, 0.25, distToClipPlane);
#endif

#if DEBUG_SHOW_DEPTH
    return SampleSceneZ_Eye(UNITY_PROJ_COORD(i.projPos)) * _ProjectionParams.w;
#endif

#if DEBUG_SHOW_NOISE3D
    return Noise3D_GetFactorFromUVW(i.uvwNoise.xyz);
#endif

    half cameraIsOrtho = unity_OrthoParams.w; // w is 1.0 when camera is orthographic, 0.0 when perspective
    float pixDistFromSource = length(i.posObjectSpace.z);

    // Vector Camera to current Pixel, in object space and normalized
    float3 vecCamToPixOSN = normalize(i.posObjectSpace.xyz - _CameraPosObjectSpace);

    // Deal with ortho camera:
    // With ortho camera, we don't want to change the fresnel according to camera position.
    // So instead of computing the proper vector "Camera to Pixel", we take account of the "Camera Forward" vector (which is not dependant on the pixel position)
    float3 vecCamForwardOSN = _CameraParams.xyz;

    vecCamToPixOSN = lerp(vecCamToPixOSN, vecCamForwardOSN, cameraIsOrtho);

#if VLB_NOISE_3D || !OPTIM_VS
    // Blend inside and outside
    float vecCamToPixDotZ = dot(vecCamToPixOSN, float3(0, 0, 1));
    float factorNearAxisZ = abs(vecCamToPixDotZ);
#endif

    // Noise factor
#if VLB_NOISE_3D
    {
        float noise3DFactor = Noise3D_GetFactorFromUVW(i.uvwNoise_intensity.rgb);

        // disable noise 3D when looking from behind or from inside because it makes the cone shape too much visible
        noise3DFactor = lerp(noise3DFactor, 1, pow(factorNearAxisZ, 10));

        intensity *= noise3DFactor;
    }
#endif

    // depth blend factor
#if VLB_DEPTH_BLEND
    {
        // we disable blend factor when the pixel is near the light source,
        // to prevent from blending with the light source model geometry (like the flashlight model).
        float depthBlendStartDistFromSource = _DepthBlendDistance;
        float depthBlendDist = _DepthBlendDistance * invLerpClamped(0, depthBlendStartDistFromSource, pixDistFromSource);
        float depthBlendFactor = DepthFade_PS_BlendDistance(i.projPos, depthBlendDist);
        depthBlendFactor = lerp(depthBlendFactor, 1, step(_DepthBlendDistance, 0));
        depthBlendFactor = lerp(depthBlendFactor, 1, cameraIsOrtho); // disable depth BlendState factor with ortho camera (temporary fix)
        intensity *= depthBlendFactor;
    }
#endif

    float3 posViewSpace = i.posViewSpace_extraData.xyz;

#if !OPTIM_VS
    {
        float isCap = i.posViewSpace_extraData.w;

        // no intensity for cap if _DrawCap = 0
        intensity *= step(isCap - 0.00001, _DrawCap);

        // Treat Cap a special way: cap is only visible from inside
        intensity *= 1 - outsideBeam * isCap; // AKA if (outsideBeam == 1 && isCap == 1) intensity = 0

        // boost factor
        float boostFactor = ComputeBoostFactor(pixDistFromSource, outsideBeam, isCap);

        // fresnel
        intensity *= ComputeFresnel(i.posObjectSpace, vecCamToPixOSN, outsideBeam, boostFactor);

        // attenuation
        intensity *= ComputeAttenuation(pixDistFromSource, _DistanceFadeStart, _DistanceFadeEnd, _AttenuationLerpLinearQuad);

        // fade when too close to camera factor
        float fadeWithCameraEnabled = 1 - max(boostFactor,      // do not fade according to camera when we are in boost zone, to keep boost effect
                                              cameraIsOrtho);   // fading according to camera eye position doesn't make sense with ortho camera
        intensity *= ComputeFadeWithCamera(posViewSpace, fadeWithCameraEnabled);

        // smooth blend between inside and outside geometry depending of View Direction
        intensity *= ComputeInOutBlending(vecCamToPixDotZ, outsideBeam);

#if DEBUG_BLEND_INSIDE_OUTSIDE
        return lerp(float4(1, 0, 0, 1), float4(0, 1, 0, 1), ComputeInOutBlending(vecCamToPixDotZ, outsideBeam));
#endif // DEBUG_BLEND_INSIDE_OUTSIDE
    }
#else // OPTIM_VS ON
    {
        float boostFactor = i.posViewSpace_extraData.w;

        // fade when too close to camera factor
        float fadeWithCameraEnabled = 1 - max(boostFactor,      // do not fade according to camera when we are in boost zone, to keep boost effect
                                              cameraIsOrtho);   // fading according to camera eye position doesn't make sense with ortho camera
        intensity *= ComputeFadeWithCamera(posViewSpace, fadeWithCameraEnabled);

#if !OPTIM_VS_FRESNEL_VS
        // compute fresnel in fragment shader to keep good quality even with low tessellation
        intensity *= ComputeFresnel(i.posObjectSpace, vecCamToPixOSN, outsideBeam, boostFactor);
#endif
    }
#endif // OPTIM_VS

    // Do not fill color.rgb only, because of performance drops on android
#if !OPTIM_VS
    half4 color = ComputeColor(pixDistFromSource, outsideBeam);
#else
    half4 color = i.color;
#endif

#if ALPHA_AS_BLACK
    color *= intensity;
    UNITY_APPLY_FOG_COLOR(i.fogCoord, color, fixed4(0, 0, 0, 0)); // since we use this shader with Additive blending, fog color should be treated as black
#else
    color.a *= intensity;
    UNITY_APPLY_FOG(i.fogCoord, color);
#endif
    return color;
}

#endif
