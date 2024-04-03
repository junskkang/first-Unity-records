using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�߻� Ŭ������ �������̽� Ŭ����

//�߻� Ŭ���� : ��� �޴� ���ϵ� Ŭ���� �ʿ��� �����Լ��� ������ �ǹ�ȭ �ϴ� Ŭ����

//abstract class Weapon  //���� �����Լ��� �ϳ��� �����ϴ� Ŭ������ �߻�Ŭ������ �Ѵ�.
//{
//    //public virtual void Attack()  //�Ϲ� �����Լ��� ���ϵ� �ʿ��� �������̵��� ���ص� �ȴ�.
//    //{

//    //}

//    public abstract void Attack();  //���� �����Լ�
//    //public, protected�� ����, private�� �Ұ���
//}
public class Item
{
    public int m_Level = 1;
    public int m_Star = 3;
    public int m_Grade = 4;

    public void ShowInfo()
    {
        Debug.Log($"�����۷���({m_Level}) �����ۼ���({m_Star}) �����۵��({m_Grade})");
    }

}

public abstract class Inven
{

}

interface Weapon
{
    //private int price = 1000;   //�������̽� Ŭ������ �Ϲݺ����� ���� �� ����.

    int Price { get; set; }     //������Ƽ�� ����� ���� �����ϴ�

    void Attack(); //���ټ����� �����ϸ� �⺻�� public�Ӽ�
}

interface Armor
{
    void Defense();
}

class Knife : Item, Weapon, Armor
{
    int m_Price = 1000;
    public int Price   //������Ƽ�� �ǹ������� �������� �Ѵ�.
    {
        get { return m_Price; }
        set { m_Price = value; }  //value : set ������ ���� �Ϲ��� �Ű�����
    }
    
    public int m_Power = 10;
    

    public void PrintInfo()
    {
        Debug.Log(m_Power);
    }

    public void Attack()  //�߻� Ŭ������ ��ӹ��� Ŭ������ �ݵ�� �������̵��� �ؾ� �Ѵ�.
    {
        Debug.Log("Į�� ����");
    }

    public void Defense()
    {

    }

}

public class Test_1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Weapon a_Wp = new Weapon();  //�߻��Լ��� ������ �ִ� Ŭ������ �ν��Ͻ��� ���� �� ����.

        Weapon a_Wp2 = new Knife();  //��ĳ������ �̿��� ����� ����!

        Knife a_Knife = new Knife();
        a_Knife.PrintInfo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
