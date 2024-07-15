

Shader "Custom/TextureAndColorShader"
{
    Properties
    {
       _MyColor ("Main Color", COLOR) = (0, 0, 1, 1)    
       //_MyColor �������� C#��ũ��Ʈ���� �Ȱ��� �̸����� �����ؼ� ������ �� ����

       _MainTex ("Base Texture", 2D)= "white" {}
    }
    SubShader
    {
        Pass
        {            
            Material
            {
                Diffuse [_MyColor]    //���� ���� ���� ����
                Ambient [_MyColor]    //��ο� �κ��� ����
            }
            Lighting On                 //����Ʈ�� �ް� ��

            SetTexture [_MainTex] {Combine texture * primary DOUBLE}
            //primary ����Ʈ��
        }
    }
    FallBack "Diffuse"
}
