Shader "Custom/Egg_Lit_Shader"
{
	Properties
	{
		_RimColor("Rim Color", color) = (0.36,0.36,0.36,1)
		_RimPower("Rim Power", Range(0.1,10.0)) = 0.82

		[Space(10)]

	_DLightG("Detail_Light_Picker", Color) = (0,1,0,1)
		_DLightReplace("Detail_Light", Color) = (0,1,0,1)

		[Space(10)]

	_DMidR("Detail_Mid_Picker", Color) = (1,0,0,1)
		_DMidReplace("Detail_Mid", Color) = (1,0,0,1)

		[Space(10)]

	_DDarkB("Detail_Dark_Picker", Color) = (0,0,1,1)
		_DDarkReplace("Detail_Dark", Color) = (0,0,1,1)

		[Space(10)]

	_DAltM("Detail_Alt_Picker", Color) = (1,0,1,1)
		_DAltReplace("Detail_Alt", Color) = (1,0,1,1)

		[Space(10)]

		_DetailTex("Detail (RGBA)", 2D) = "white" {}

	//chroma
	_thresh("Threshold", Range(0, 16)) = 0.8
		_slope("Slope", Range(0, 1)) = 0.2
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		//make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float3 normal : TEXCOORD1;
		float4 world : TEXCOORD2;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
	};

	sampler2D _DetailTex;

	float4 _DetailTex_ST;

	uniform float _RimPower;
	uniform float4 _RimColor;
	half _TexPower;

	//chroma
	float _thresh; // 0.8
	float _slope; // 0.2

	float4 _DLightG, _DLightReplace;
	float4 _DMidR, _DMidReplace;
	float4 _DDarkB, _DDarkReplace;
	float4 _DAltM, _DAltReplace;

	float edge0;

	fixed4 _GColorT;
	fixed4 _GColorM;
	fixed4 _GColorB;
	float  _Middle;

	v2f vert(appdata v)
	{
		v2f o;
		o.world = mul(unity_ObjectToWorld, v.vertex);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _DetailTex);
		o.normal = UnityObjectToWorldNormal(v.normal);

		return o;
	}

	float calculateSmoothstep(float4 keyColor, float4 input_color)
	{
		float Mask = abs(length(abs(keyColor.rgb - input_color.rgb)));
		return smoothstep(edge0, _thresh, Mask);
	}

	float4 calculateCleanMask(float _smoothstep, float4 replacementColor)
	{
		return (1 - _smoothstep)*replacementColor;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float4 input_color = tex2D(_DetailTex, i.uv).rgba;

		//Lighting
		//float3 viewDir = normalize(_WorldSpaceCameraPos);				
		float3 viewDir = normalize(_WorldSpaceCameraPos - i.world);
		float dotProduct = 1 - dot(normalize(i.normal), viewDir);
		float3 rimColor = pow(smoothstep(1 - _RimPower, 1.0, dotProduct), 3);
		rimColor *= _RimColor;

		edge0 = _thresh * (1.0 - _slope);

		float DLightGSmoothstep = calculateSmoothstep(_DLightG, input_color);
		float DMidRSmoothstep = calculateSmoothstep(_DMidR, input_color);
		float DDarkBSmoothstep = calculateSmoothstep(_DDarkB, input_color);
		float DAltMSmoothstep = calculateSmoothstep(_DAltM, input_color);

		float4 input_colorMask = (DLightGSmoothstep *
			DMidRSmoothstep *
			DDarkBSmoothstep *
			DAltMSmoothstep) * input_color;

		float4 FinalColour = (calculateCleanMask(DLightGSmoothstep, _DLightReplace) +
			calculateCleanMask(DMidRSmoothstep, _DMidReplace) +
			calculateCleanMask(DDarkBSmoothstep, _DDarkReplace) +
			calculateCleanMask(DAltMSmoothstep, _DAltReplace));

		FinalColour.rgb += rimColor;


		return float4(FinalColour.rgb, 1);
	}
		ENDCG
	}
	}
		FallBack "VertexLit"
}
