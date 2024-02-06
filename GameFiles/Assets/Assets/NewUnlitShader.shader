Shader "Custom/PinkGlowingGrid"
{
    Properties
    {
        _GridColour ("Grid Colour", Color) = (1,0,1,1)
        _GridSize ("Grid Size", Range(0.01, 1.0)) = 0.1
        _GridLineThickness ("Grid Line Thickness", Range(0.00001, 0.010)) = 0.003
        _Glow ("Glow", Range(0, 1)) = 0.5
        _GlowIntensity ("Emission Intensity", Range(0,5)) = 1
    }
    SubShader
    {
        Tags  {"Queue" = "Transparent" "RenderType" = "Transparent" } 
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
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

            float4 _GridColour;
            float _GridSize;
            float _GridLineThickness;
            float _Glow;
            float _GlowIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Normalized UVs for the grid pattern
                float2 uv = i.uv;
                uv = uv / _GridSize - float2(0.5,0.5) * _GridSize;

                // Calculate grid pattern
                float2 grid = abs(frac(uv) - 0.5);
                float gridPattern = smoothstep(_GridLineThickness, _GridLineThickness + _Glow, min(grid.x, grid.y));

                // Calculate glow effect
                float glow = 1.0 - smoothstep(0.0, _Glow * _GridSize, min(grid.x, grid.y));

                // Combine grid color with glow
                fixed4 col = _GridColour;
                col.rgb *= glow * _GlowIntensity; // Multiply color by glow intensity
                col.a = gridPattern * _GlowIntensity; // Use grid pattern for alpha

                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
