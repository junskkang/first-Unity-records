using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//C# 자료구조(C#자료구조 라이브러리 : Generic Collections)
//C# Generic Collection 중 List 
public class Test_1 : MonoBehaviour
{
    // 정렬 조건 함수
    int Comp(int a, int b)       //매개변수는 해당 리스트의 노드 데이터형
    {                            //알파벳순 : 매개변수를 char형으로 
        return a.CompareTo(b);   //오름차순(ASC) : 낮은값에서 높은값으로 정렬
        //return b.CompareTo(a);   //내림차순(DESC) : 높은값에서 낮은값으로 정렬
    }


    // Start is called before the first frame update
    void Start()
    {
        // List 사용법
        List<int> a_List = new List<int>();

        //Debug.Log("노드 추가 하기");
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

        //foreach (int val in a_List)    //변수명 인덱스에 담긴 값을 하나씩 꺼내서 복사해서 출력하겠다는 의미
        //        Debug.Log(val);

        //Debug.Log("중간값 제거");
        //a_List.RemoveAt(1);        //1번 인덱스 노드 제거 방법
        ////for (int ii = 0; ii < a_List.Count; ii++)
        ////    Debug.Log($"a_List[{ii}] : {a_List[ii]}");

        //Debug.Log("마지막노드 제거");
        //if(0 < a_List.Count)
        //    a_List.RemoveAt(a_List.Count - 1);

        for (int ii = 0; ii < a_List.Count; ii++)
            Debug.Log($"a_List[{ii}] : {a_List[ii]}");

        Debug.Log("중간값 추가");
        a_List.Insert(1, 10);
        a_List.Insert(3, 30);

        int a_Idx = 0;
        foreach (int val in a_List)
        { 
            Debug.Log($"a_List[{a_Idx}] : {val}");
            a_Idx++;
        }

        Debug.Log("정렬하기");
        a_List.Sort(Comp);    //정렬조건함수를 입력하지 않을시 디폴트값은 오름차순

        foreach (int val in a_List)
        {
            Debug.Log($"a_List[{a_Idx}] : {val}");
            a_Idx++;
        }

        Debug.Log("전체 노드 삭제하기");
        a_List.Clear();
        Debug.Log("노드의 갯수 : " + a_List.Count);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
