Shader "Unlit/TestNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_base v)
            {
                v2f o;

				float l = 10;
				float w =  2 * UNITY_PI / l;
				float f = w * _Time.y * 3;
				v.vertex.y = sin(v.vertex.x * w + f);

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal + sin(v.vertex.x * w + f);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float dotF = saturate(dot(i.normal,fixed3(1,1,1)));
                fixed4 col = tex2D(_MainTex, i.uv) * dotF;
                return col;
            }
            ENDCG
        }
    }
}
