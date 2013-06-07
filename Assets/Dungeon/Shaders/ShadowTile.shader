Shader "Custom/ShadowTile" 
{
	Properties 
	{
		_Color ("Main Color", Color) = (1,1,1,1)	
	}

	SubShader 
	{
		Cull back
		
		//Draw SOLID transparent pixels
   		Pass 
   		{
   			Lighting On
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			
       		Material {
                Diffuse [_Color]
				Ambient [_Color]
            }
		}
	}
}