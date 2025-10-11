// Simple Unlit shader that creates a smooth falloff from the center of a mesh.
// Perfect for soft-edged circles, glows, or hazy effects.
Shader "Unlit/SmoothFalloff"
{
// Properties that will be exposed in the Unity Inspector
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0, 1) // The main color of the shape
        _InnerBounds ("Inner Bounds", Range(0, 1)) = 0.5 // The size of the solid inner area
        _Softness ("Softness", Range(0, 1)) = 0.5 // The smoothness of the edge falloff
    }
    SubShader
    {
        // Set up for transparency
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend One OneMinusSrcAlpha // Standard alpha blending
        ZWrite Off // Don't write to the depth buffer
        Cull Off // Render both sides of the mesh

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // Struct to pass data from the vertex shader to the fragment shader
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

            // Properties defined above are accessible here
            fixed4 _Color;
            float _InnerBounds;
            float _Softness;

            // Vertex Shader: Calculates the screen position of each vertex
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Pass the UV coordinates to the fragment shader
                return o;
            }

            // Fragment Shader: Calculates the color of each pixel
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from the center of the UVs (0.5, 0.5) for each axis
                // and scale it to a 0-1 range where 0 is center and 1 is the edge.
                float2 distFromCenter = abs(i.uv - 0.5) * 2.0;
                
                // Use the maximum of the two distances. This creates a square-shaped distance field.
                float maxDist = max(distFromCenter.x, distFromCenter.y);

                // Use smoothstep to create a smooth transition from opaque to transparent
                // It will return 0 if maxDist is >= _InnerBounds + _Softness
                // It will return 1 if maxDist is <= _InnerBounds
                // It will smoothly interpolate between 0 and 1 for distances between these two values
                float alpha = 1.0 - smoothstep(_InnerBounds, _InnerBounds + _Softness, maxDist);

                // The final color is the property color with the calculated alpha
                fixed4 finalColor = _Color;
                finalColor.a *= alpha;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}