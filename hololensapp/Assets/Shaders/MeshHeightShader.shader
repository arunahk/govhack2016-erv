//Darrin Hurd - This will render hydration in 2 passes back then front 

Shader "Trimble/MeshHeight" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_GridTex ("Grid (RGB) Trans (A)", 2D) = "white" {}
	_TextureScale ("TextureScale", Range (0.001, 200)) = 1  //slider for scaling the texture - 1 = 1 unit in world space
	_Blend ("Blend", Range (0.0, 20)) = 10
	_Height ("Height", Range (-20, 20)) = 0
	_Height2 ("Height2", Range (-20, 20)) = 0
}

SubShader {
	//Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Geometry"}
	
	LOD 200
	//Blend SrcAlpha OneMinusSrcAlpha
	Cull Back

	CGPROGRAM
	#pragma surface surf Lambert 

	sampler2D _MainTex;
	sampler2D _GridTex;
	fixed4 _Color;
	float _Blend;
	float _Height;
	float _Height2;
	float _TextureScale;

	struct Input {
		float2 uv_MainTex;
		float3 worldNormal;
		float3 worldPos;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		float2 UV;
		fixed4 c;
		fixed4 g;
		fixed4 z;

		//IN.worldNormal = float3(0,0,1);
			UV = IN.worldPos.xy; // front



		//c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		c = tex2D(_GridTex, UV/_TextureScale);
		z = (0,0,0,0);


			float blended = (IN.worldPos.y)/ (_Height2- _Height);
			if (blended>1) blended = 1;
			if (blended<0) blended = 0;
			//blended = 0.1;
			
			c = tex2D(_GridTex, float2(0.5f, blended));

			
			
			//c.rgba = lerp (c.rgba, z.rgba,  0.5f);



	
		o.Albedo = c.rgb;
		o.Alpha = 1;
	
	}
	ENDCG


	}





Fallback "Legacy Shaders/Transparent/VertexLit"
}
