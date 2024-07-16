Shader "Custom/2TextureWithColor"
{
    Properties
    {
        _Color ("Main Color", COLOR) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _SubTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent"}
        Pass 
        {
            Material
            {
                Diffuse [_Color]
                Ambient [_Color]
            }
            Lighting On

            Blend SrcAlpha OneMinusSrcAlpha

            SetTexture [_MainTex] 
            {
                Combine texture
            }

            SetTexture [_SubTex]
            {
                //ConstantColor [_Color]
                //Combine texture lerp(texture) previous, constant
                Combine texture lerp(texture) previous
                //_SubTex�� ���İ��� 0�� �������� previous�� �׸���, 1�� �������� texture�� �׸�
            }
            SetTexture [_SubTex]
            {
                Combine previous * primary DOUBLE    
            }
        }
    }    
    FallBack "Diffuse"
}
