using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEvent : MonoBehaviour
{
    Monster_Ctrl refMonCS;
    void Start()
    {
        refMonCS = transform.parent.GetComponent<Monster_Ctrl>();
    }

    // Update is called once per frame
    //void Update()
    //{

    //}
    //애니메이션이 있는 오브젝트에서 애니메이션 이벤트로 호출하여
    //몬스터컨트롤로 넘겨줌
    void Event_AttHit() 
    {
        refMonCS.Event_AttHit();
    }
}
