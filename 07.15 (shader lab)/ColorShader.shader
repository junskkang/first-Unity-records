

Shader "Custom/ColorShader"
{
    Properties
    {
       _MyColor ("Main Color", COLOR) = (0, 0, 1, 1)    
       //_MyColor 변수명은 C#스크립트에서 똑같은 이름으로 접근해서 수정할 수 있음

    }
    SubShader
    {
        Pass
        {            
            Material
            {
                Diffuse [_MyColor]    //빛을 받은 곳의 색깔
                Ambient [_MyColor]    //어두운 부분의 색깔
            }
            Lighting On                 //라이트를 받게 함
        }
    }
    FallBack "Diffuse"
}
