using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//C#의 자료구조
//Dictionary
//특징 : 키 값과 밸류로 구성되어 있는 자료구조 키 값을 통해 검색이 빠른 구조이다.
//       인덱스로 문자열을 사용할 수 있다.
public class Test_2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dictionary<string, string> a_Dt = new Dictionary<string, string>();
        a_Dt["Apple"] = "사과";
        a_Dt["Strawberry"] = "딸기";
        a_Dt["Banana"] = "바나나";
        a_Dt["Watermelon"] = "수박";
        a_Dt["Orange"] = "오렌지";


        //순환 출력
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

        //검색
        Debug.Log(a_Dt["Banana"]);

        // ContainsKey(키값) 해당 키값이 존재하는지 확인하는 함수
        if (a_Dt.ContainsKey("Mango") == true)  
            Debug.Log(a_Dt["Mango"]);

        a_Dt["Apple"] = "멜론";   //이미 존재하는 키값에 대입해주면 Value(내용)이 바뀐다.

        a_Idx = 0;
        foreach (KeyValuePair<string, string> a_Node in a_Dt)
        {
            Debug.Log($"[{a_Idx}] : Key({a_Node.Key}), Value({a_Node.Value})");
            a_Idx++;
        }

        //삭제
        a_Dt.Remove("Banana");

        Debug.Log("삭제 후 결과 확인");
        a_Idx = 0;
        foreach (KeyValuePair<string, string> a_Node in a_Dt)
        {
            Debug.Log($"[{a_Idx}] : Key({a_Node.Key}), Value({a_Node.Value})");
            a_Idx++;
        }

        //전체 삭제
        a_Dt.Clear();
        Debug.Log("전체삭제 후 결과 확인");
        Debug.Log("전체 삭제 결과 확인 : " + a_Dt.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
