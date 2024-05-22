using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AllyType
{
    BT_Ally,        //아군
    BT_Enemy,       //적군

}

public class BulletCtrl : MonoBehaviour
{
    [HideInInspector] public AllyType m_AllyType = AllyType.BT_Ally;
    //이동 관련 변수
    Vector3 m_DirVec = Vector3.zero;    //날아갈 방향 벡터
    Vector3 m_StartPos = new Vector3(0, 0, 1);  //스폰 위치 계산용 변수

    Vector3 m_MoveStep = Vector3.zero; //한프레임 당 이동 벡터 계산용 변수
    float m_MoveSpeed = 35.0f;   //총알 이동속도

    //사거리 관련 변수
    float m_ShootRange = 30.0f; //사거리

    [HideInInspector] public float m_Damage = 10.0f;

    [HideInInspector] public bool m_IsPool = false; 
    // == true : 메모리풀로 관리되는 총알 SetActive 스위칭
    // == false : Instantiate, Destroy로 생성 파괴를 반복하는 총알
    void Start()
    {
        
    }

    
    void Update()
    {
        //날아가야 할 방항으로 이동시키기
        m_MoveStep = m_DirVec * (Time.deltaTime * m_MoveSpeed);
        m_MoveStep.y = 0.0f;

        transform.Translate(m_MoveStep, Space.World);

        //총알 삭제
        Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);
        if (a_Pos.x < -0.1f || 1.1f < a_Pos.x || a_Pos.y < -0.1f || 1.1f < a_Pos.y)
        {
            if (m_IsPool == true)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
        else
        {//총알의 사거리 제한
            float a_Length = Vector3.Distance(transform.position, m_StartPos);
            // == float a_Length = (transform.position - m_StartPos).magnitude;
            if (m_ShootRange < a_Length)
            {
                if (m_IsPool == true)
                    gameObject.SetActive(false);
                else
                    Destroy(gameObject);
            }                
        }

        ////참고! 반대로 화면 바깥에서 화면 안쪽으로 스폰되는 투사체 
        //Vector3 a_SPos = Vector3.zero;
        //a_SPos.x = 1.1f;
        //a_SPos.y = Random.Range(0.0f, 1.0f);
        //Vector3 a_CacPos = Camera.main.ViewportToWorldPoint(a_SPos);

    }

    public void BulletSpawn(Vector3 a_OwnPos, Vector3 a_DirVec, float a_ShootRange = 30.0f, float a_Dmg = 10)
    {
        //날아가야 할 방향 벡터 구하기
        a_DirVec.y = 0.0f;
        m_DirVec = a_DirVec;
        m_DirVec.Normalize();

        //캐릭터의 위치 기준에서 날아가게끔 하기
        m_StartPos = a_OwnPos + (m_DirVec * 2.5f);  //총알 날아가는 위치를 캐릭터보다 살짝 앞쪽으로
        m_StartPos.y = transform.position.y;

        transform.position = new Vector3 (m_StartPos.x, transform.position.y, m_StartPos.z);

        //총알이 날아가는 방향으로 바라보게 회전 시켜주기
        //transform.rotation = Quaternion.LookRotation(m_DirVec);
        transform.forward = m_DirVec;

        //사거리 저장 
        //캐릭터에서 함수를 호출하며 매개변수로 넘어온 a_ShootRange를 저장 
        m_ShootRange = a_ShootRange;

        int cri = Random.Range(0, 10);
        if (cri < 6)
            m_Damage = a_Dmg;
        else
            m_Damage = a_Dmg * 2;

        //m_AllyType = a_AllyType;
    }
}
