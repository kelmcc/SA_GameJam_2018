Shader "Custom/FoodBits_Unlit_Shader"
{
	Properties
	{
		_RimColor("Rim Color", color) = (0.36,0.36,0.36,1)
		_RimPower("Rim Power", Range(0.1,10.0)) = 0.82

		[Space(10)]

		_Color("Color", Color) = (0,1,0,1)
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
		float4 vertex : SV_POSITION;
	};

	sampler2D _DetailTex;


	uniform float _RimPower;
	uniform float4 _RimColor;
	half _TexPower;


	float4 _Color;

	v2f vert(appdata v)
	{
		v2f o;
		o.world = mul(unity_ObjectToWorld, v.vertex);
		o.vertex = UnityObjectToClipPos(v.vertex);
		//o.uv = TRANSFORM_TEX(v.uv, _DetailTex);
		o.normal = UnityObjectToWorldNormal(v.normal);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{

		//Lighting
		//float3 viewDir = normalize(_WorldSpaceCameraPos);				
		float3 viewDir = normalize(_WorldSpaceCameraPos - i.world);
		float dotProduct = 1 - dot(normalize(i.normal), viewDir);
		float3 rimColor = pow(smoothstep(1 - _RimPower, 1.0, dotProduct), 3);
		rimColor *= _RimColor;

		_Color.rgb += rimColor;


		return float4(_Color.rgb, 1);
	}
		ENDCG
	}
	}
		FallBack "VertexLit"
}
