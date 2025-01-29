Shader "Custom/InvisibleIfStencil"
{
    Properties
    {
        // Existing properties...

        // New property for parent object's position
        _ParentPosition("Parent Position", Vector) = (0, 0, 0, 0)
    }

    SubShader
    {
        // Existing SubShader...

        Pass
        {
            // Existing Pass...

            HLSLPROGRAM
            #pragma target 2.0

            // Vertex shader input structure
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // Vertex shader output structure
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1; // To pass world position to fragment
            };

            // Vertex shader
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex); // Calculate world position
                o.uv = v.uv;
                return o;
            }

            // Fragment shader
            void frag(v2f i, inout SurfaceOutputStandard o)
            {
                // Compare the object's parent's world position with the invisibility threshold
                if (i.worldPos.z < _ParentPosition.z) // Use the Z axis
                {
                    o.Alpha = 0.0; // Make the object invisible
                }
                else
                {
                    o.Alpha = o.Alpha; // Keep the original alpha
                }
            }

            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
