Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-1" }

        Pass
        {
            Stencil
            {
                Ref 1
                Comp always
                Pass replace
            }

            ColorMask 0 // Don't actually render anything
        }
    }
}
