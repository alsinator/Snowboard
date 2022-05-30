Shader "Custom/SpriteSwapColors"{

	Properties{
		_Color("Tint", Color) = (0, 0, 0, 1)
		_MainTex("Texture", 2D) = "white" {}
	
		_HairColor("HairColor", Color) = (0, 0, 0, 1)
		_HairTint("HairTint", Color) = (0, 0, 0, 1)

		_ShirtColor("ShirtColor", Color) = (0, 0, 0, 1)
		_ShirtTint("ShirtTint", Color) = (0, 0, 0, 1)

		_PantsColor("PantsColor", Color) = (0, 0, 0, 1)
		_PantsTint("PantsTint", Color) = (0, 0, 0, 1)

	}

		SubShader{
			Tags{
				"RenderType" = "Transparent"
				"Queue" = "Transparent"
			}

			Blend SrcAlpha OneMinusSrcAlpha

			ZWrite off
			Cull off

			Pass{

				CGPROGRAM

				#include "UnityCG.cginc"

				#pragma vertex vert
				#pragma fragment frag

				sampler2D _MainTex;
				float4 _MainTex_ST;

				fixed4 _Color;

				fixed4 _HairColor;
				fixed4 _HairTint;
				fixed4 _ShirtColor;
				fixed4 _ShirtTint;
				fixed4 _PantsColor;
				fixed4 _PantsTint;


				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					fixed4 color : COLOR;
				};

				struct v2f {
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed4 color : COLOR;
				};

				v2f vert(appdata v) {
					v2f o;
					o.position = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.color = v.color;
					return o;
				}

				fixed4 SwapColor(fixed4 col, fixed4 original, fixed4 swap)
				{

					fixed3 delta = abs(col.rgb - original.rgb);
					fixed3 rgb = length(delta) < 0.1 ? swap.rgb : col.rgb;
					return fixed4(rgb, col.a);
				}


				fixed4 frag(v2f i) : SV_TARGET{
					fixed4 col = tex2D(_MainTex, i.uv);
					
					col = SwapColor(col, _HairColor, _HairTint);
					col = SwapColor(col, _ShirtColor, _ShirtTint);
					col = SwapColor(col, _PantsColor, _PantsTint);

					col *= _Color;
					col *= i.color;
					return col;
				}

				ENDCG
			}
	}
}
