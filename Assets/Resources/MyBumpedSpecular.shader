Shader "Custom/XRay"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_BumpTex("_AfterTex", 2D) = "white" {}
		_Power("Power", Color) = (1,1,1,1)
	}

		SubShader
	{
		Tags{ "RenderType" = "transparent" }
		LOD 200
		pass
	{
		CGPROGRAM
#pragma vertex vert      
#pragma fragment frag      
		sampler2D _MainTex;
		sampler2D _AfterTex;
		float4  _AfterColor;

		struct appdata
		{
			float4 vertex : POSITION;
			float4 texcoord : TEXCOORD0;
			float3 normal : NORMAL;
		};

		struct v2f
		{
			float4 pos : POSITION;
			float4 uv : TEXCOORD0;
			float2 uvLight : TEXCOORD1;
		};

		v2f vert(appdata v)
		{
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
			o.uv = v.texcoord;
			half2 nor;
			nor.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
			nor.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
			o.uvLight = nor * 0.5 + 0.5;
			return o;
		}
		float4 frag(v2f i) : COLOR
		{
			float4 col = tex2D(_MainTex, i.uv);
			float4 lightCol = tex2D(_AfterTex, i.uvLight);
			return col + lightCol * 1.0 * _AfterColor;
		}
			ENDCG
	}
	}
}