Shader "Custom/VertexColored" 
{
	SubShader {
        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
        }

        Pass {
        }
    }

	Fallback " VertexLit", 1
}