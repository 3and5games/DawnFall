// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Projector/Light" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_CenterColor("Center Color", Color) = (1,1,1,1)
		_ShadowTex ("Cookie", 2D) = "" {}
		_CenterTex("Center", 2D) = "" {}
		_FalloffTex ("FallOff", 2D) = "" {}
		_Power("Power", float) = 1
	}
	
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor One
			Offset -1, -1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			#include "UnityCG.cginc"
			
			struct v2f {
				float2 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 _Color;
			fixed4 _CenterColor;
			float _Power;
			sampler2D _ShadowTex;
			float4 _ShadowTex_ST;
			sampler2D _CenterTex;
			float4 _CenterTex_ST;
			sampler2D _FalloffTex;

			fixed4 frag (v2f i) : SV_Target
			{
				i.uvShadow = TRANSFORM_TEX (i.uvShadow, _ShadowTex);

				fixed4 texS = tex2D (_ShadowTex, i.uvShadow).rgba*_Color;
				fixed4 texF = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff));

				fixed4 texCS = tex2D(_CenterTex, i.uvShadow).rgba*_CenterColor;

				fixed4 res = texS * texF.a*_Color.a*texS.a + texCS * texF.a * _CenterColor.a*texCS.a;

				UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(0,0,0,0));
				return res*_Power;
			}
			ENDCG
		}
	}
}
