using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Value Type의 변수: int, float, double, bool ...... struct의 변수
// Reference Type의 변수 : array, Class객체

// ref, out : Value Type의 변수들을 Reference Type처럼 동작 시켜주는 키워드
// ref : 유연함 (매개변수로 넘겨 받은 변수는 메서드 안에서 사용하지 않아도 됨)
// out : 엄격함 (매개변수로 넘겨 받은 변수는 메서드 안에서 반드시 사용해야 함)
public class Test_3 : MonoBehaviour
{
    void ValueMethod(int a_ii)
    {
        a_ii = 1000;
    }
    void RefMethod(ref int a_ii)
    {
        //a_ii = 1000;
        //Debug.Log("Test : " + a_ii);
    }

    void OutMethod(out int a_ii)
    {
        a_ii = 1000;
        Debug.Log("Test : " + a_ii);
    }
    bool Add(int a, int b, ref int x, ref float y, ref long z)
    {
        if(x < 0)
        {
            return false;
        }

        x = x + 100;
        y = x * 2.5f;
        z = x * 10000;

        return true;
    }
    // Start is called before the first frame update
    void Start()
    {
        // Value Type의 변수 대입은 단순 복사
        int aaa = 0;
        int bbb = aaa;
        bbb = 999;
        Debug.Log(aaa + " : " + bbb);   // 0 : 999

        int xxx = 0;
        ref int vvv = ref xxx;     // ref 키워드를 붙여주면 일반변수도 참조형태로 됨
        ref int yyy = ref xxx;
        ref int zzz = ref xxx;
        vvv = 999;
        Debug.Log(xxx + " : " + vvv + " : " + yyy + " : " + zzz);   // 999 : 999 : 999 : 999



        int ccc = 0;
        ValueMethod(ccc);
        Debug.Log("ccc" + ccc);          // 0

        int eee = 0;
        RefMethod(ref eee);
        Debug.Log("eee" + eee);          // 1000

        int ddd = 0;
        OutMethod(out ddd);
        Debug.Log("ddd" + ddd);          // 1000


        int a_Int = 5;
        float a_Float = 0.0f;
        long a_Long = 0L;
        if(Add(1, 2,ref a_Int, ref a_Float, ref a_Long) == true)
            Debug.Log(a_Int + " : " + a_Float + " : " + a_Long);
        
            
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
