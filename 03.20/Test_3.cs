using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Value Type�� ����: int, float, double, bool ...... struct�� ����
// Reference Type�� ���� : array, Class��ü

// ref, out : Value Type�� �������� Reference Typeó�� ���� �����ִ� Ű����
// ref : ������ (�Ű������� �Ѱ� ���� ������ �޼��� �ȿ��� ������� �ʾƵ� ��)
// out : ������ (�Ű������� �Ѱ� ���� ������ �޼��� �ȿ��� �ݵ�� ����ؾ� ��)
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
        // Value Type�� ���� ������ �ܼ� ����
        int aaa = 0;
        int bbb = aaa;
        bbb = 999;
        Debug.Log(aaa + " : " + bbb);   // 0 : 999

        int xxx = 0;
        ref int vvv = ref xxx;     // ref Ű���带 �ٿ��ָ� �Ϲݺ����� �������·� ��
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
