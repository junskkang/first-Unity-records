using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//C# �ڷᱸ��(C#�ڷᱸ�� ���̺귯�� : Generic Collections)
//C# Generic Collection �� List 
public class Test_1 : MonoBehaviour
{
    // ���� ���� �Լ�
    int Comp(int a, int b)       //�Ű������� �ش� ����Ʈ�� ��� ��������
    {                            //���ĺ��� : �Ű������� char������ 
        return a.CompareTo(b);   //��������(ASC) : ���������� ���������� ����
        //return b.CompareTo(a);   //��������(DESC) : ���������� ���������� ����
    }


    // Start is called before the first frame update
    void Start()
    {
        // List ����
        List<int> a_List = new List<int>();

        //Debug.Log("��� �߰� �ϱ�");
        a_List.Add(111);
        a_List.Add(222);
        a_List.Add(333);

        //for(int ii = 0; ii < a_List.Count; ii++)
        //{
        //    Debug.Log(a_List[ii]);
        //}
        //Debug.Log("-----------");

        a_List.Add(444);
        a_List.Add(555);

        //for (int ii = 0; ii < a_List.Count; ii++)
        //{
        //    Debug.Log(a_List[ii]);
        //}

        //foreach (int val in a_List)    //������ �ε����� ��� ���� �ϳ��� ������ �����ؼ� ����ϰڴٴ� �ǹ�
        //        Debug.Log(val);

        //Debug.Log("�߰��� ����");
        //a_List.RemoveAt(1);        //1�� �ε��� ��� ���� ���
        ////for (int ii = 0; ii < a_List.Count; ii++)
        ////    Debug.Log($"a_List[{ii}] : {a_List[ii]}");

        //Debug.Log("��������� ����");
        //if(0 < a_List.Count)
        //    a_List.RemoveAt(a_List.Count - 1);

        for (int ii = 0; ii < a_List.Count; ii++)
            Debug.Log($"a_List[{ii}] : {a_List[ii]}");

        Debug.Log("�߰��� �߰�");
        a_List.Insert(1, 10);
        a_List.Insert(3, 30);

        int a_Idx = 0;
        foreach (int val in a_List)
        { 
            Debug.Log($"a_List[{a_Idx}] : {val}");
            a_Idx++;
        }

        Debug.Log("�����ϱ�");
        a_List.Sort(Comp);    //���������Լ��� �Է����� ������ ����Ʈ���� ��������

        foreach (int val in a_List)
        {
            Debug.Log($"a_List[{a_Idx}] : {val}");
            a_Idx++;
        }

        Debug.Log("��ü ��� �����ϱ�");
        a_List.Clear();
        Debug.Log("����� ���� : " + a_List.Count);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
