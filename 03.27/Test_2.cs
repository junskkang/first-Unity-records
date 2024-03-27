using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//C#�� �ڷᱸ��
//Dictionary
//Ư¡ : Ű ���� ����� �����Ǿ� �ִ� �ڷᱸ�� Ű ���� ���� �˻��� ���� �����̴�.
//       �ε����� ���ڿ��� ����� �� �ִ�.
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, string> a_Dt = new Dictionary<string, string>();
        a_Dt["Apple"] = "���";
        a_Dt["Strawberry"] = "����";
        a_Dt["Banana"] = "�ٳ���";
        a_Dt["Watermelon"] = "����";
        a_Dt["Orange"] = "������";


        //��ȯ ���
        int a_Idx = 0;
        //string a_StrBuff = "";
        foreach (KeyValuePair<string, string> a_Node in a_Dt)
        {
            Debug.Log($"[{a_Idx}] : Key({a_Node.Key}), Value({a_Node.Value})");
            a_Idx++;
        }

        KeyValuePair<string, string> a_PrNode;
        for (int i = 0; i < a_Dt.Count; i++)
        {
            a_PrNode = a_Dt.ElementAt(i);     //using System.Linq;
            Debug.Log($"[{i}] : Key({a_PrNode.Key}), Value({a_PrNode.Value})");
        }

        //�˻�
        Debug.Log(a_Dt["Banana"]);

        // ContainsKey(Ű��) �ش� Ű���� �����ϴ��� Ȯ���ϴ� �Լ�
        if (a_Dt.ContainsKey("Mango") == true)  
            Debug.Log(a_Dt["Mango"]);

        a_Dt["Apple"] = "���";   //�̹� �����ϴ� Ű���� �������ָ� Value(����)�� �ٲ��.

        a_Idx = 0;
        foreach (KeyValuePair<string, string> a_Node in a_Dt)
        {
            Debug.Log($"[{a_Idx}] : Key({a_Node.Key}), Value({a_Node.Value})");
            a_Idx++;
        }

        //����
        a_Dt.Remove("Banana");

        Debug.Log("���� �� ��� Ȯ��");
        a_Idx = 0;
        foreach (KeyValuePair<string, string> a_Node in a_Dt)
        {
            Debug.Log($"[{a_Idx}] : Key({a_Node.Key}), Value({a_Node.Value})");
            a_Idx++;
        }

        //��ü ����
        a_Dt.Clear();
        Debug.Log("��ü���� �� ��� Ȯ��");
        Debug.Log("��ü ���� ��� Ȯ�� : " + a_Dt.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
