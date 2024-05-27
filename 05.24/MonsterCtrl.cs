using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonsterType
{
    Zombi, Missile, Boss
}

public class MonsterCtrl : MonoBehaviour
{
    public MonsterType type = MonsterType.Zombi;
    
    public GameObject shootPos = null;

    public Image HpBar;
    float curHp = 200.0f;
    float maxHp = 200.0f;

    float moveSpeed = 6.0f;
    Vector3 moveVec = Vector3.zero;
    Vector3 spawnPos = Vector3.zero;

    float cacPosY = 0.0f;   //이동용 Sin함수에 들어갈 각도 계산용 변수
    float ranY = 0.0f;      //랜덤한 진폭값 저장용 변수
    float cycleSpeed = 0.0f;//랜덤한 진동 속도 변수

    void Start()
    {
        spawnPos = transform.position;
        ranY = Random.Range(0.5f, 2.0f);    //Sin함수의 랜덤 진폭
        cycleSpeed = Random.Range(1.8f, 5.0f);  //진동수 랜덤값
    }

    
    void Update()
    {
        if (type == MonsterType.Zombi)
        {
            ZombiAIUpdate();
        }

        if (transform.position.x < CameraResolution.m_ScWMin.x - 2.0f) Destroy(gameObject);

    }

    void ZombiAIUpdate()
    {
        moveVec = transform.position;
        moveVec += Vector3.left * moveSpeed * Time.deltaTime;
        cacPosY += (Time.deltaTime * cycleSpeed);
        moveVec.y = spawnPos.y + Mathf.Sin(cacPosY) * ranY;
        transform.position = moveVec;

        if (transform.position.y < CameraResolution.m_ScWMin.y + 2.0f)   //화면의 아래쪽끝
            moveVec.y = CameraResolution.m_ScWMin.y + 2.0f;

        if (CameraResolution.m_ScWMax.y - 2.0f < transform.position.y)   //화면의 위쪽끝
            moveVec.y = CameraResolution.m_ScWMax.y - 2.0f;

        transform.position = moveVec;
    }

    public void TakeDamage(float value)
    {
        if (curHp <= 0.0f) return;

        curHp -= value;
        if (curHp <= 0.0f)
            curHp = 0.0f;   

        if(HpBar != null)
            HpBar.fillAmount = curHp / maxHp;

        if (curHp <= 0.0f)
        {
            Destroy(gameObject); 
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "AllyBullet")
        {
            TakeDamage(80.0f);
            Destroy(coll.gameObject);
        }
    }

}
