using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

//Ŭ���� �Ҽ��� static(����) ����, �����޼���
//Ư¡
//1. ��ü �������� Ŭ�����̸�.������, Ŭ�����̸�.�޼����̸�();
//2. ���α׷��� ������ �� �޸𸮰� Ȯ���ǰ� ���α׷��� ����� ������ �����ȴ�.
//3. Ŭ���� �Ҽ������� �޸𸮸� Ŭ������ ������ �����ϰ� �����ȴ�.

public class Hero
{
    public string m_Name;    //�Ϲ� ��� ����(�ν��Ͻ� ��� ����) 
    public int m_Hp;         //�ν��Ͻ��� �־������ ����� �� �ִ� ������� �ǹ�

    public static int s_UserPoint = 0;  //���� ��� ���� (Ŭ���� ��� ����)

    public void AddUserPoint(int a_Point)   //�Ϲ� ��� �޼���
    {
        s_UserPoint += a_Point;
    }

    public int GetUserPoint()               //�Ϲ� ��� �޼���
    {
        return s_UserPoint;
    }

    public static void StaticPrint()    //���� ��� �޼��� (Ŭ���� ��� �޼���)
    {
       //m_Name = "";                  //Ŭ���� ��� �޼��忡�� �Ϲ� ��������� �ȵ�!

        int a_ABC = 100;                //���� ����
        s_UserPoint = 1234;             //���� ��� ���� ��� �ٷ� ����
        Debug.Log(s_UserPoint);
    }

}
public class Test_1 : MonoBehaviour
{

    void Start()
    {
#if KOREA
        Debug.Log("�̰��� �ѱ� �����Դϴ�.");
#elif ENGLISH
        Debug.Log("�̰��� ���� �����Դϴ�.");
#elif CHINA
        Debug.Log("�̰��� �߱��� �����Դϴ�.");
#endif
        Hero.s_UserPoint = 100;    // ���� ��� ������ ���
        Hero.StaticPrint();        // ���� ��� �޼����� ���

        //����Ƽ���� �������ִ� static method ����
        Debug.Log("");
        Random.Range(0, 100);
        Vector3 a_Vec = Vector3.zero;
        


        // 3:3 �� ��� RPG�� �� ĳ���� 3���� �����ؼ� ���ӿ� �� ����
        Hero hunter = new Hero();
        hunter.m_Name = "��ɲ�";
        hunter.m_Hp = 123;
        Debug.Log(hunter.m_Name + " : " + hunter.m_Hp);
        //hunter.s_UserPoint = 100; //static ������ ��ü�� �����ϴ°� �ƴϾ ������
        hunter.AddUserPoint(100);

        Hero Warrior = new Hero();
        Warrior.m_Name = "����";
        Warrior.m_Hp = 200;
        Debug.Log(Warrior.m_Name + " : " + Warrior.m_Hp);
        

        Hero healer = new Hero();
        healer.m_Name = "����";
        healer.m_Hp = 80;
        Debug.Log(healer.m_Name + " : " + healer.m_Hp);
        healer.AddUserPoint(30);
        

        Debug.Log(Hero.s_UserPoint + " : " + hunter.GetUserPoint() + Warrior.GetUserPoint() + healer.GetUserPoint());

    }


    void Update()
    {
        
    }
}
