// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Color" {
   Properties {
      _MainTex ("RGBA Texture Image", 2D) = "white" {} 
      _Cutoff ("Accuracy", Float) = 0.012
	  _TransparentColor ("Transparent Color",Color) = (0,0,0,1)
   }
   SubShader {
      Pass {    
         Cull Back // since the front is partially transparent, 
            // we shouldn't cull the back

         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
        
 
         uniform sampler2D _MainTex;    
         uniform float _Cutoff;
		 uniform float4 _TransparentColor;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.tex = input.texcoord;
            output.pos = UnityObjectToClipPos(input.vertex);
            return output;
         }

         float4 frag(vertexOutput input) : COLOR
         {
            float4 textureColor = tex2D(_MainTex, input.tex.xy);  
			float4 diff = textureColor - _TransparentColor;
			if ((abs(diff.r)<_Cutoff)&&(abs(diff.g)<_Cutoff)&&(abs(diff.b)<_Cutoff))
            {
               discard;
            }
			else
			{
				textureColor = textureColor;
			}
            return textureColor;
         }
 
         ENDCG
      }
   }
   Fallback "Unlit/Transparent Color"
}