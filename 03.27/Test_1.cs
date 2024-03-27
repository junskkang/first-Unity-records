using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//����Ƽ C# �ڷᱸ��
// List, Stack, Queue
// 1. ����
// �뵵 : �ӽ� �����
// Ư¡ : �ϳ��� �ؿ������� �������� �װ�, ������ ���������� �ϳ��� ��. �߰����� �ȵ�
// ���� : TCG���� ī�� ���� �׿����� �� ���������� ������ ���� ���

// 2. ť
// �뵵 : �ӽ� �����, �޸��� ����
// Ư¡ : ���Լ���
// ���� : ��Ʈ��ũ ��ſ��� ��Ŷ�̶�� ������ �޾Ƴ��� ó���ϴ� ���
//        ä���� �ϴٰ� ���ͳ��� ���ڱ� ����ٰ� ����Ǹ�
//        �׵��� �׿��ִ� ä���� ��� ���� ����Ǿ� �ִٰ�
//        �Էµ� ������� �Ҹ��� ������ ��.
public class Test_1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //����
        Stack<int> a_Stk = new Stack<int>();
        a_Stk.Push(10);
        a_Stk.Push(20);
        a_Stk.Push(30);
        a_Stk.Push(3);
        a_Stk.Push(50);

        int a_Idx = 0;
        while (0 < a_Stk.Count)
        {
            int a_Svalue = a_Stk.Pop();
            Debug.Log($"[{a_Idx}] : {a_Svalue}");
            a_Idx++;
        }

        Debug.Log("������ ���� : " +a_Stk.Count);


        //ť
        Queue<float> a_Que = new Queue<float>();
        a_Que.Enqueue(1.1f);
        a_Que.Enqueue(2.2f);
        a_Que.Enqueue(3.3f);
        a_Que.Enqueue(4.4f);
        a_Que.Enqueue(5.5f);

        a_Idx = 0;
        while (0 < a_Que.Count)
        {
            float a_QValue = a_Que.Dequeue();
            Debug.Log($"[{a_Idx}] : {a_QValue}");
            a_Idx++;

        }
        Debug.Log("ť�� ���� ���� : " + a_Que.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
