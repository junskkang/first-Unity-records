using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//유니티 C# 자료구조
// List, Stack, Queue
// 1. 스택
// 용도 : 임시 저장소
// 특징 : 하나씩 밑에서부터 차곡차곡 쌓고, 뺄때는 위에서부터 하나씩 뺌. 중간빼기 안돼
// 예시 : TCG에서 카드 덱이 쌓여있을 때 위에서부터 뒤집어 보는 경우

// 2. 큐
// 용도 : 임시 저장소, 메모리의 버퍼
// 특징 : 선입선출
// 예시 : 네트워크 통신에서 패킷이라는 정보를 받아놓고 처리하는 경우
//        채팅을 하다가 인터넷이 갑자기 끊겼다가 연결되면
//        그동안 쌓여있던 채팅이 어느 곳에 저장되어 있다가
//        입력된 순서대로 쫘르륵 나오게 됨.
public class Test_1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //스택
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

        Debug.Log("스택의 갯수 : " +a_Stk.Count);


        //큐
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
        Debug.Log("큐의 남은 갯수 : " + a_Que.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
