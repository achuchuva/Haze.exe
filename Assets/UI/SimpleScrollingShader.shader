// Simple Unlit shader that scrolls a texture.
// Designed for 2D backgrounds, with controls for speed, direction, and tiling.
Shader "Unlit/SimpleScrollingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tiling ("Tiling (X, Y)", Vector) = (1, 1, 0, 0)
        _Speed ("Scroll Speed", Range(0, 10)) = 1.0
        _Rotation ("Scroll Direction (Degrees)", Range(0, 360)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // A library that contains common helper functions.
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST; // For tiling and offset, Unity auto-populates this.
            float2 _Tiling;
            float _Speed;
            float _Rotation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Apply the tiling from the material's Tiling/Offset fields
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // Apply the custom _Tiling property
                o.uv *= _Tiling;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Convert rotation from degrees to radians for math functions
                float rotationRadians = _Rotation * (3.14159265359 / 180.0);

                // Calculate direction vector from rotation
                float2 direction = float2(cos(rotationRadians), sin(rotationRadians));

                // Calculate the UV offset based on time, speed, and direction
                float2 offset = direction * _Time.y * _Speed;

                // Apply the offset to the UV coordinates
                float2 scrolledUV = i.uv + offset;

                // Sample the texture with the new, scrolled UV coordinates
                fixed4 col = tex2D(_MainTex, scrolledUV);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}