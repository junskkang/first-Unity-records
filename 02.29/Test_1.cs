using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //1 ~ 10 문제풀이
        //int Hap = 0;
        //int idx = 1;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;  // Hap += idx;
        //idx = 2;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 3;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 4;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 5;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 6;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 7;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 8;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 9;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);
        //Hap = Hap + idx;
        //idx = 10;
        //Debug.Log((Hap + idx) + " = " + Hap + " + " + idx);

        int Hap = 0;
        for(int idx=1; idx<=10; idx++)
        {
            Debug.Log((Hap+idx)+" = "+Hap+" + "+ idx);
            Hap = Hap + idx; // Hap += idx;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
