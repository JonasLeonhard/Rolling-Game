Shader "ErbGameArt/Particles/Blend_Noise" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,0.5)
        _MainTexUspeed ("MainTex U speed", Float ) = 0
        _MainTexVspeed ("MainTex V speed", Float ) = 0
        _Noise ("Noise", 2D) = "white" {}
        _NoiseUspeed ("Noise U speed", Float ) = 0
        _NoiseVspeed ("Noise V speed", Float ) = 0
        _Emission ("Emission", Float ) = 2
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _NoiseUspeed;
            uniform float _NoiseVspeed;
            uniform float _MainTexUspeed;
            uniform float _MainTexVspeed;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Emission;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 node_3878 = ((float2((_MainTexUspeed*(1-i.uv0.b)),((1-i.uv0.b)*_MainTexVspeed))*_Time.g)+i.uv0);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_3878, _MainTex));
                float2 node_691 = ((float2(_NoiseUspeed,_NoiseVspeed)*_Time.g)+i.uv0);
                float4 _Noise_var = tex2D(_Noise,TRANSFORM_TEX(node_691, _Noise));
                float3 emissive = (_MainTex_var.rgb*_Noise_var.rgb*i.vertexColor.rgb*_TintColor.rgb*_Emission);
                fixed4 finalRGBA = fixed4(emissive,(_MainTex_var.a*_Noise_var.a*i.vertexColor.a*_TintColor.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
}
