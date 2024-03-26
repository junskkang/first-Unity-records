using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GawiBawiBo   // 
{
    Gawi = 1,
    Bowi = 2,
    Bo = 3    //������ ���ҿ��� ��ǥ�� �ᵵ �����ص� ����
}

public enum City  //���ҵ鿡 ���� ���� ���� ���
{
    Seoul,        //0   �ڵ����� ������� 0���� ����
    Inchoen,      //1
    Busan = 5,    //5   ���� �� ������ �ٽ� 1�� ����
    Gwangju,      //6
    Jeju ,     //7
    Shinchon,     //8
    Count         //9 
}

public class Test_1 : MonoBehaviour
{

    void Start()
    {
        City MyCity = City.Busan;
        Debug.Log("My City : "+ MyCity + " : Index(" + (int)MyCity + ")");
        Debug.Log("Shinchon : " + " : Index(" + (int)City.Shinchon + ")");

        //int a_UserSel = 1;   //1�̸� ������ �ǹ���
        //int a_ComSel = Random.Range(1, (3 + 1));

        //if (a_UserSel == a_ComSel)
        //    Debug.Log("�����ϴ�");
        //else if (a_UserSel == 1 && a_ComSel == 3)
        //    Debug.Log("�̰���ϴ�");
        //else
        //    Debug.Log("�����ϴ�");


        //���� �ڵ带 enum������ 
        GawiBawiBo a_UserSel = GawiBawiBo.Gawi;
        GawiBawiBo a_ComSel = (GawiBawiBo)Random.Range((int)GawiBawiBo.Gawi,
             (int)GawiBawiBo.Bo + 1);

        if (a_UserSel == a_ComSel)
            Debug.Log("�����ϴ�");
        else if (a_UserSel == GawiBawiBo.Gawi && a_ComSel == GawiBawiBo.Bo)
            Debug.Log("�̰���ϴ�");
        else
            Debug.Log("�����ϴ�");
    }

    void Update()
    {
        
    }
}
