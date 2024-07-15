

Shader "Custom/ColorShader"
{
    Properties
    {
       _MyColor ("Main Color", COLOR) = (0, 0, 1, 1)    
       //_MyColor �������� C#��ũ��Ʈ���� �Ȱ��� �̸����� �����ؼ� ������ �� ����

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
        }
    }
    FallBack "Diffuse"
}
