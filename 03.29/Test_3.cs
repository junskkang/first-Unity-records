using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ŭ���� �������̵� (�� �����ε�) : virtual, override Ű����
// ��� ���迡 �ִ� �θ��� �Լ����� �ڽ� Ŭ������ ���� �Լ������� �����ؼ� �������Ͽ� ����ϴ� ��
//<�������̵��� �ϱ� ���� ����>
//1. �Լ��� �̸��� ���ƾ� �Ѵ�.
//2. �Ű������� ���ƾ� �Ѵ�.
//3. ��ȯŸ���� ���ƾ� �Ѵ�.
//4. �θ�Ŭ������ virtual Ű���尡 �ʿ��ϴ�.


//��ĳ���ð� �ٿ�ĳ���� : (=����ȯ)
//class Animal {}
//class Dog : Animal {}

//Dog dog = new Dog();
//Animal animal = dog;     // ��ĳ����. �ڽ��� ��ü�� �θ��� ��ü�� ������ �� ����

//Animal a_Animal2 = new Animal();//�ٿ�ĳ����. �θ��� ��ü�� �ڽ��� ��ü�� ������ �� ����
//Dog a_Dog2 = (Dog)a_Animal2;  // ��� ����
//Dog a_Down = (Dog)animal;    //��ĳ���� �Ǿ��ִ� ��ü�� �ٽ� �ٿ�ĳ���� �����ϴ�.

class MyParent
{
    public int m_ii;

    public MyParent()
    {
        Debug.Log("MyParent�� ������ �Լ� ȣ��");

    }

    public void Method()
    {
        Debug.Log("MyParent�� Method() ȣ��");
    }
    public virtual void MyPrint()
    {
        Debug.Log("�θ� Ŭ������ MyPrint()�޼��� ȣ��");
    }
}

class MyBoy : MyParent
{
    public MyBoy()
    {
        m_ii = 10;
    }

    public new void Method()
    {//new Ű���带 ����Ͽ� �Լ��� �������̵��� ����� �� �ִ�.
        Debug.Log("MyBoy�� Method() ȣ��");
    }

    public override void MyPrint()
    {
        Debug.Log("MyBoy Ŭ������ MyPrint()�޼��� ȣ��");
    }

}

class MyDaughter : MyParent
{
    public MyDaughter()
    {

    }

    public override void MyPrint()
    {
        Debug.Log("MyDaughter Ŭ������ MyPrint()�޼��� ȣ��");
    }

}

public class Test_3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyParent a_pp = new MyParent();  //"�θ� Ŭ������ MyPrint()�޼��� ȣ��"
        a_pp.MyPrint();

        MyBoy xy = new MyBoy(); //"MyBoy Ŭ������ MyPrint()�޼��� ȣ��"
        xy.MyPrint();

        MyDaughter xx = new MyDaughter(); //"MyDaughter Ŭ������ MyPrint()�޼��� ȣ��"
        xx.MyPrint();

        MyParent a_yy = xy;           //��ĳ����. �ڽ��� �θ�ü�� ����
        a_yy.MyPrint();       //"MyBoy Ŭ������ MyPrint()�޼��� ȣ��"

        a_pp.Method();       //�θ� Ŭ������ �Լ� ȣ��

        xy.Method();         // �Ƶ� Ŭ������ �Լ� ȣ��

        a_yy.Method();       //(��ĳ���õ� ��) �θ� Ŭ������ �Լ� ȣ��

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
