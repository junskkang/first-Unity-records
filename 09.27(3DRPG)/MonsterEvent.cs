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
    //�ִϸ��̼��� �ִ� ������Ʈ���� �ִϸ��̼� �̺�Ʈ�� ȣ���Ͽ�
    //������Ʈ�ѷ� �Ѱ���
    void Event_AttHit() 
    {
        refMonCS.Event_AttHit();
    }
}
