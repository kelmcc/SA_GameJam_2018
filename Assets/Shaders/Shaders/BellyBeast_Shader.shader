//Author: Kelly

Shader "Custom/BellyBeast_Shader"
{
	Properties
	{
		//_Contrast("Contrast", Range(0.001,1)) = 0.1
		[Space(10)]

		_ShadowColorHue("Shadow Color Hue", Range(0,360)) = 0.03
		_ShadowColorSat("Shadow Color Saturation", Range(-1,1)) = 0
		_ShadowColorBri("Shadow Color Brightness", Range(-1 ,1)) = 0.22

		_ShadowTestColor("Shadow Test Color", Color) = (1,0,0,1)
			//_ShadowPower("Shadow Power", Range(0.001,1)) = 0.1
			[Space(10)]

			_RimColor("Rim Color", color) = (0.36,0.36,0.36,1)
			_RimPower("Rim Power", Range(0.1,10.0)) = 0.82
			[Space(5)]
			_GColorT("Gradient Color Top", Color) = (1,0,0,1)
			_GColorM("Gradient Color Mid", Color) = (0,1,0,1)
			_GColorB("Gradient Color Bottem", Color) = (0,0,1,1)
			_Middle("Middle", Range(0.001, 0.999)) = 0.001

			_MainTex("Gradient", 2D) = "white" {}

			[Space(30)]

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

			_DColourC("Detail_Colour_Picker", Color) = (0,0,0,1)
			_DColourReplace("Detail_Colour", Color) = (0,0,0,1)

			[Space(10)]

			_DColourC2("Detail_Colour2_Picker", Color) = (1, 0.92, 0.016, 1)
			_DColourC2Replace("Detail_Colour2", Color) = (1, 1, 1, 1)

				//chroma
				_thresh("Threshold", Range(0, 16)) = 0.8
					_slope("Slope", Range(0, 1)) = 0.2

	}

		SubShader
			{
				Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" }
				LOD 200

				CGPROGRAM


				// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SimpleLambert finalcolor:mycolor //fullforwardshadows

				// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

			struct Input {
				float2 uv_MainTex;
				float4 texcoord : TEXCOORD0;
				float4 pos : SV_POSITION;
				float3 viewDir;
				float3 worldNormal;
			};

			struct SurfaceOutputCustom {
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				half Specular;
				fixed Gloss;
				fixed Alpha;
				float3 RimColor;
			};
			sampler2D _MainTex;

			//chroma
			float _thresh; // 0.8
			float _slope; // 0.2

			float4 _DLightG, _DLightReplace;
			float4 _DMidR, _DMidReplace;
			float4 _DDarkB, _DDarkReplace;
			float4 _DAltM, _DAltReplace;
			float4 _DColourC, _DColourReplace;
			float4 _DColourC2, _DColourC2Replace;

			//uniform float _Contrast;
			uniform float _ShadowColorHue;
			uniform float _ShadowColorSat;
			uniform float _ShadowColorBri;
			float3 _ShadowTestColor;
			//uniform float _ShadowPower;
			uniform float _RimPower;
			uniform float4 _RimColor;

			float4 input_color;
			float edge0;

			fixed4 _GColorT;
			fixed4 _GColorM;
			fixed4 _GColorB;
			float  _Middle;


			float calculateSmoothstep(float4 keyColor)
			{
				float Mask = abs(length(abs(keyColor.rgb - input_color.rgb)));
				return smoothstep(edge0, _thresh, Mask);
			}

			float4 calculateCleanMask(float _smoothstep, float4 replacementColor)
			{
				return (1 - _smoothstep)*replacementColor;
			}

			fixed3 ReplaceColor(fixed3 color)
			{
				color = (color.r - _ShadowTestColor.r*(2 * color.r - color.g - color.b),
					color.g - _ShadowTestColor.g*(2 * color.g - color.r - color.b),
					color.b - _ShadowTestColor.b*(2 * color.b - color.r - color.g))* _ShadowTestColor;

				return color;
			}

			float3 RGBConvertToHSV(float3 rgb)
			{
				float R = rgb.x, G = rgb.y, B = rgb.z;
				float3 hsv;
				float max1 = max(R, max(G, B));
				float min1 = min(R, min(G, B));
				if (R == max1)
				{
					hsv.x = (G - B) / (max1 - min1);
				}
				if (G == max1)
				{
					hsv.x = 2 + (B - R) / (max1 - min1);
				}
				if (B == max1)
				{
					hsv.x = 4 + (R - G) / (max1 - min1);
				}
				hsv.x = hsv.x * 60.0;
				if (hsv.x < 0)
					hsv.x = hsv.x + 360;
				hsv.z = max1;
				hsv.y = (max1 - min1) / max1;
				return hsv;
			}

			float3 HSVConvertToRGB(float3 hsv)
			{
				float R, G, B;
				//float3 rgb;
				if (hsv.y == 0)
				{
					R = G = B = hsv.z;
				}
				else
				{
					hsv.x = hsv.x / 60.0;
					int i = (int)hsv.x;
					float f = hsv.x - (float)i;
					float a = hsv.z * (1 - hsv.y);
					float b = hsv.z * (1 - hsv.y * f);
					float c = hsv.z * (1 - hsv.y * (1 - f));
					//switch statement doesnt work on IOS
		#if SHADER_API_GLES

					if (i == 0)
					{
						R = hsv.z; G = c; B = a;
					}
					else if (i == 1)
					{
						R = b; G = hsv.z; B = a;
					}
					else if (i == 2)
					{
						R = a; G = hsv.z; B = c;
					}

					else if (i == 3)
					{
						R = a; G = b; B = hsv.z;
					}
					else if (i == 4)
					{
						R = c; G = a; B = hsv.z;
					}
					else
					{
						R = hsv.z; G = a; B = b;
					}

		#else
					switch (i)
					{
					case 0: R = hsv.z; G = c; B = a;
						break;
					case 1: R = b; G = hsv.z; B = a;
						break;
					case 2: R = a; G = hsv.z; B = c;
						break;
					case 3: R = a; G = b; B = hsv.z;
						break;
					case 4: R = c; G = a; B = hsv.z;
						break;
					default: R = hsv.z; G = a; B = b;
						break;
					}
		#endif
				}

				return float3(R, G, B);
			}


			half4 LightingSimpleLambert(SurfaceOutputCustom s, half3 lightDir, half atten)
			{
				half NdotL = dot(s.Normal, lightDir);
				float diff = clamp((NdotL* atten), 0.0, 1.0);
				diff = saturate(lerp(0.5, diff, 1));
				float3 Hsv = RGBConvertToHSV(s.Albedo);
				Hsv = float3(Hsv.x - _ShadowColorHue, Hsv.y - _ShadowColorSat, Hsv.z - _ShadowColorBri);
				float3 ShadowGradient = lerp(HSVConvertToRGB(Hsv), s.Albedo, diff);

				half4 c;
				//c.rgb = s.Albedo * (_LightColor0.rgb)* (NdotL * atten);
				c.rgb = ShadowGradient.rgb;

				c.a = s.Alpha;
				return c;
			}

			void mycolor(Input IN, SurfaceOutputCustom o, inout fixed4 color)
			{
				color.rgb = saturate(color.rgb) + o.RimColor;
			}

			void surf(Input IN, inout SurfaceOutputCustom  o)
			{
				//Detail Colour keyer
				input_color = tex2D(_MainTex, IN.uv_MainTex).rgba;
				edge0 = _thresh * (1.0 - _slope);

				float DLightGSmoothstep = calculateSmoothstep(_DLightG);
				float DMidRSmoothstep = calculateSmoothstep(_DMidR);
				float DDarkBSmoothstep = calculateSmoothstep(_DDarkB);
				float DAltMSmoothstep = calculateSmoothstep(_DAltM);
				float DColourCSmoothstep = calculateSmoothstep(_DColourC);
				float DColourC2Smoothstep = calculateSmoothstep(_DColourC2);

				float4 input_colorMask = (DLightGSmoothstep *
					DMidRSmoothstep *
					DDarkBSmoothstep *
					DAltMSmoothstep *
					DColourCSmoothstep *
					DColourC2Smoothstep) * input_color;

				float4 DetailsColor = (calculateCleanMask(DLightGSmoothstep, _DLightReplace) +
					calculateCleanMask(DMidRSmoothstep, _DMidReplace) +
					calculateCleanMask(DDarkBSmoothstep, _DDarkReplace) +
					calculateCleanMask(DAltMSmoothstep, _DAltReplace) +
					calculateCleanMask(DColourCSmoothstep, _DColourReplace) +
					calculateCleanMask(DColourC2Smoothstep, _DColourC2Replace));


				//Gradient
				fixed4 Gradient = lerp(_GColorB, _GColorM, IN.uv_MainTex.y / _Middle) * step(IN.uv_MainTex.y, _Middle);
				Gradient += lerp(_GColorM, _GColorT, (IN.uv_MainTex.y - _Middle) / (1 - _Middle)) * step(_Middle, IN.uv_MainTex.y);
				Gradient.a = 1;

				float dp = dot(normalize(IN.viewDir), normalize(IN.worldNormal));

				float3 rimColor = pow(smoothstep(1 - _RimPower, 1.0, 1 - dp), 3);
				rimColor *= _RimColor;

				float3 FinalColour = ((1 - DetailsColor.a) * Gradient) + DetailsColor;

				o.Albedo = saturate(FinalColour);
				o.RimColor = saturate(rimColor);
			}

			ENDCG
			}
				FallBack "Diffuse"
}