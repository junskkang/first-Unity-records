using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMsCtrl : MonoBehaviour
{
    float moveSpeed = 12.0f;     //이동속도
    float rotSpeed = 100.0f;    //회전속도
    //일부러 타겟을 향해 날아가는 각도를 비틀어서 점차적으로 따라가는 것처럼 보이게 하는 효과

    //유도탄 변수
    [HideInInspector] public GameObject targetObj = null;   //타겟참조 변수
    Vector3 desireDir = Vector3.zero;   //타겟을 향하는 방향


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObj == null)  //타겟 오브젝트가 없으면
            FindEnemy();        //타겟을 찾아주는 함수

        if (targetObj != null)  //타겟 오브젝트가 있으면
            BulletHoming();     //타겟을 향해 날아가는 추적 행동 함수
        else
            transform.Translate(transform.right * moveSpeed * Time.deltaTime, Space.World);
    }

    void FindEnemy()
    {
        //몬스터 태그를 달고 있는 게임오브젝트 찾아오는 함수
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Monster");

        if (enemyList.Length <= 0) return; //등장해 있는 몬스터가 하나도 없으면 리턴

        GameObject findMonster = null;
        Vector3 cacVec = Vector3.zero;
        MonsterCtrl refMonster = null;
        for (int i = 0; i < enemyList.Length; i++)
        {
            refMonster = enemyList[i].GetComponent<MonsterCtrl>();  //

            if (refMonster == null) continue;   //몬스터 컨트롤 없으면 빠꾸

            //if (refMonster.homingLockOn != null) continue;
            if (refMonster.homingLockOn1 != null && refMonster.homingLockOn2 != null) continue;
            //락온 되어 있으면 빠꾸

            findMonster = enemyList[i].gameObject;

            //if (refMonster.homingLockOn == null)
            //    refMonster.homingLockOn = this.gameObject;

            if (refMonster.homingLockOn1 == null) //한마리당 최대 2개의 미사일 타겟 가능
                refMonster.homingLockOn1 = this.gameObject;
            else
                refMonster.homingLockOn2 = this.gameObject;

            break;
        }

        targetObj = findMonster;    //호밍미사일의 타겟에다가 락온 한 몬스터 객체를 대입

    }

    void BulletHoming()
    {
        //타겟 대상과 호밍미사일을 향하는 벡터 구하기
        desireDir = targetObj.transform.position - transform.position;  
        desireDir.z = 0.0f;
        desireDir.Normalize();

        //타겟을 향해 유도탄 회전시키기
        //Atan2 : 기준으로부터 각도를 구하는 함수 첫번째 변수를 기준으로 두번째 매개변수까지의 각도
        float angle = Mathf.Atan2(desireDir.y, desireDir.x) * Mathf.Rad2Deg;
        //XY축 평면이기 때문에 Z축 방향을 가리키는 Vector3.forward를 기준으로 회전시켜줘야 함
        Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);    //각도를 쿼터니온으로

        //첫번째 매개변수의 로테이션 값을 두번째 매개변수 값만큼 세번째 매개변수의 속도로 회전시키는 함수
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                     targetRot, rotSpeed * Time.deltaTime);

        //유도탄이 바라보는 방향쪽으로 움직이게 하기
        transform.Translate(transform.right * moveSpeed * Time.deltaTime, Space.World);

    }
}
