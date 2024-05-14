using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    //키보드 이동 관련 변수
    float h, v;                   //키보드 입력값을 받기 위한 변수
    float m_MoveSpeed = 10.0f;    //초당 이동속도

    Vector3 m_DirVec;             //이동하려는 방향 벡터 변수


    //좌표 계산용 변수들
    Vector3 m_CurPos;
    Vector3 m_CacEndVec;

    //총알 발사 관련 변수
    float m_AttSpeed = 0.1f;    //공격속도
    float m_CacAttTick = 0.0f;  //기관총 발사 주기 만들기
    float m_ShootRange = 30.0f; //사거리

    //캐릭터 화면 벗어나기 금지용 변수
    float maxX = 0.98f;
    float minX = 0.02f;
    float maxY = 0.93f;
    float minY = 0.08f;

    //JoyStick 이동 처리 변수
    float m_JoyMvLen = 0.0f;                //조이스틱 땡기는 힘
    Vector3 m_JoyMvDir = Vector3.zero;      //조이스틱 땡기는 방향

    //마우스 클릭 이동 관련 변수 (Mouse Picking Move)
    [HideInInspector] public bool m_bMoveOnOff = false; //현재 마우스 피킹으로 이동 중인지
    Vector3 m_TargetPos;    //마우스 피킹 목표점
    float m_CacStep;    //한스탭 계산용 변수
    
    Vector3 m_PickVec = Vector3.zero;
    public ClickMark m_ClickMark;

    //체력 관련 변수
    [HideInInspector] public float m_MaxHp = 200.0f;
    [HideInInspector] public float m_CurHp = 200.0f;
    public Image HpBarImg;

    //애니메이션 관련 변수
    AnimSequence m_AnimSeq;
    Quaternion m_CacRot;

    //닉네임 관련 변수
    public Text m_Nickname;
    
    void Start()
    {
        //차일드 중 첫번째로 나오는 AnimSequence.cs 파일 찾아오기
        m_AnimSeq = gameObject.GetComponentInChildren<AnimSequence>();

        if (m_Nickname != null)
            m_Nickname.text = PlayerPrefs.GetString("UserNick", "나 강림");
    }

    
    void Update()
    {
        MousePickCtrl();

        KeyBoardUpdate();       //키보드이동 1순위

        JoyStickMvUpdate();     //조이스틱이동 2순위

        MousePickUpdate();      //마우스이동 3순위

        //LimitMove();            //바깥으로 못나가게 체크

        //총알 발사 코드
        if ( 0.0f < m_CacAttTick)       //총알 장전
            m_CacAttTick -= Time.deltaTime;

        if (Input.GetMouseButton(1) == true)    //마우스 우클릭 총알 발사
        {
            if (m_CacAttTick <= 0.0f)
            {
                //마우스의 좌표를 월드좌표계로 받아서 그 벡터를 매개변수로 넣음
                ShootFire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                m_CacAttTick = m_AttSpeed;
            }
        }

        //Bomb 스킬
        if (Input.GetKeyDown(KeyCode.R) == true)
        {
            UseeBombSkill();
        }
        //애니메이션 셋팅
        //조이스틱 움직임도 없고, 키보드 움직임도 없고, 마우스 이동도 없을 때
        if (m_JoyMvLen <= 0.0f && (0.0f == h && 0.0f == v) && m_bMoveOnOff == false)
        {
            m_AnimSeq.ChangeAniState(UnitState.Idle);
        }
        else
        {
            if (m_DirVec.magnitude <= 0.0f)
                m_AnimSeq.ChangeAniState(UnitState.Idle);
            else
            {
                //방향에 따른 애니메이션 설정하는 부분
                m_CacRot = Quaternion.LookRotation(m_DirVec);       //바라보고 있는 방향벡터를 담아줌
                m_AnimSeq.CheckAnimDir(m_CacRot.eulerAngles.y);     //방향의 y값을 넣어서 각도 체크 후 상태전환
            }
        }
    }

    #region ---- 키보드 이동
    void KeyBoardUpdate()   //키보드 이동처리
    {
        h = Input.GetAxisRaw("Horizontal"); // -1 ~ 1
        v = Input.GetAxisRaw("Vertical");   // -1 ~ 1

        if (h != 0.0f || v != 0.0f)  //이동 키보드를 조작하고 있으면
        {
            m_DirVec = (Vector3.right * h) + (Vector3.forward * v);
            if(1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();   //방향단위벡터

            //화면 바깥으로 나가지 못하도록 제어
            Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);
            if (a_Pos.x < minX)
            {
                a_Pos.x = minX + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.x > maxX)
            {
                a_Pos.x = maxX - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.y < minY)
            {
                a_Pos.y = minY + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            if (a_Pos.y > maxY)
            {
                a_Pos.y = maxY - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else
                transform.Translate(m_DirVec * m_MoveSpeed * Time.deltaTime);
        }
    }
    #endregion
    #region ---- 조이스틱 이동
    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        m_JoyMvLen = a_JoyMvLen;
        if (0.0f < a_JoyMvLen)
        {
            m_JoyMvDir = new Vector3(a_JoyMvDir.x, 0.0f, a_JoyMvDir.y);
            
            //UI 좌표는 xy좌표지만 캐릭터의 이동은 xz축으로 움직이고 있으므로
            //UI의 y좌표를 캐릭터 이동의 z축에다가 넣어주는 것
        }
    }

    public void JoyStickMvUpdate()
    {
        if (h != 0.0f || v != 0.0f)
            return;

        //조이스틱 이동코드
        if (0.0f < m_JoyMvLen)
        {
            m_DirVec = m_JoyMvDir;
            float a_MvStep = m_MoveSpeed * Time.deltaTime;
            transform.Translate(m_DirVec * m_JoyMvLen * a_MvStep, Space.Self);
        }
    }
    #endregion
    #region ---- 마우스 클릭이동
    //float m_Tick = 0.0f;
    void MousePickCtrl()    //마우스 클릭을 감지하는 함수
    {
        //if (0.0f < m_Tick)
        //    m_Tick -= Time.deltaTime;

        //if (m_Tick < 0.0f)
        //{
        //    if (Input.GetMouseButton(0) == true)    //마우스 왼쪽버튼 클릭시
        //    {
        //        m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //현재 마우스 위치 값
        //        SetMsPicking(m_PickVec);
        //        m_Tick = 0.1f;

        //    }
        //}

        if (Input.GetMouseButtonDown(0) == true &&
            GameMgr.IsPointerOverUIObject() == false)    //마우스 왼쪽버튼 클릭시
        {
            m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //현재 마우스 위치 값
            SetMsPicking(m_PickVec);

            if (m_ClickMark != null)
                m_ClickMark.PlayEff(m_PickVec, this);
        }

    }

    void SetMsPicking(Vector3 a_Pos)
    {
        Vector3 a_CacVec = a_Pos - this.transform.position; //현재 캐릭터 위치에서 타겟 위치를 향하는 벡터
        a_CacVec.y = 0;
        if (a_CacVec.magnitude < 1.0f)  //너무 짧은 거리는 이동 안시킬래
            return;

        m_bMoveOnOff = true;    //클릭으로 이동 중인 상태
        m_DirVec = a_CacVec;    
        m_DirVec.Normalize();   //방향 벡터
        m_TargetPos = new Vector3(a_Pos.x, transform.position.y, a_Pos.z);  //x와 z를 갖는 목표 지점 저장
    }

    void MousePickUpdate()
    {
        if (0.0f < m_JoyMvLen || h != 0.0f || v != 0.0f) //키보드 이동 중이면, 조이스틱 이동 중이면
            m_bMoveOnOff = false;   //마우스 이동 취소

        if (m_bMoveOnOff == true)
        {
            m_CacStep = Time.deltaTime * m_MoveSpeed;   //이번 프레임에 움직이게 될 한 걸음
            Vector3 a_CacEndVec = m_TargetPos - transform.position;
            a_CacEndVec.y = 0.0f;

            if (a_CacEndVec.magnitude <= m_CacStep)  //목표지점까지의 거리보다 보폭이 크거나 같으면 도착으로 간주
            {
                m_bMoveOnOff = false;
            }
            else
            {
                m_DirVec = a_CacEndVec;
                m_DirVec.Normalize();
                transform.Translate(m_DirVec*m_CacStep, Space.World);
            }
        }

    }
#endregion

    void LimitMove()
    {
        //월드좌표를 뷰포트좌표(0~1)로 전환
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        if (pos.x < 0.02f) pos.x = 0.02f;
        if (pos.x > 0.98f) pos.x = 0.98f;
        if (pos.y < 0.1f) pos.y = 0.1f;
        if (pos.y < 0.9f) pos.y = 0.9f;

        pos.x = Mathf.Clamp(pos.x, 0.03f, 0.97f);
        pos.y = Mathf.Clamp(pos.y, 0.07f, 0.95f);

        //다시 월드좌표로 환산
        Vector3 a_CacPos = Camera.main.ViewportToWorldPoint(pos);
        a_CacPos.y = transform.position.y;
        transform.position = a_CacPos;
    }
    public void ShootFire(Vector3 a_Pos)    //목표지점을 매개변수로 받음
    {//클릭 이벤트가 발생했을 때 함수 호출
        GameObject a_Obj = Instantiate(GameMgr.m_BulletPrefab);

        m_CacEndVec = a_Pos - transform.position;   //목표지점 - 현재캐릭터지점
        m_CacEndVec.y = 0.0f;

        BulletCtrl a_BulletSc = a_Obj.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, m_CacEndVec.normalized, m_ShootRange);
    }

    void UseeBombSkill()
    {
        if(GlobalUserData.g_BombCount <= 0) return;

        //360도 총알 발사
        Vector3 a_TargetV = Vector3.zero;
        GameObject a_NewBObj = null;
        BulletCtrl a_BL_sc = null;
        for (float Angle = 0.0f; Angle < 360.0f; Angle += 15.0f)
        {
            //원을 그리는 함수 Cos과 Sin의 순서는 방향을 시계방향으로 돌릴건지, 반시계방향으로 돌릴건지의 차이
            //x좌표 = Sin / z좌표 = Cos >> 시계방향
            //x좌표 = Cos / z좌표 = Sin >> 반시계방향
            a_TargetV.x = Mathf.Cos(Angle*Mathf.Deg2Rad);       //Deg2Rad : 상수값. Degree to Radians Converter
            a_TargetV.y = 0.0f;
            a_TargetV.z = Mathf.Sin(Angle*Mathf.Deg2Rad);
            a_TargetV.Normalize();

            a_NewBObj = Instantiate(GameMgr.m_BulletPrefab);
            a_BL_sc = a_NewBObj.GetComponent<BulletCtrl>();
            a_BL_sc.BulletSpawn(transform.position, a_TargetV, 30.0f, 120.0f);      //주인공위치 중점으로, a_TargetV 방향으로 날려라
        }
        GlobalUserData.g_BombCount--;

        GameMgr.Inst.UserInfo();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name.Contains("BulletPrefab") == true)
        {
            if (coll.gameObject.CompareTag(AllyType.BT_Ally.ToString()) == true)
                return;

            TakeDamage(5.0f);

            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.name.Contains("coin_") == true)
        {
            GameMgr.Inst.AddGold(10);
            GlobalUserData.SaveGameInfo();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.name.Contains("bomb_") == true)
        {
            
            GlobalUserData.g_BombCount++;
            GlobalUserData.SaveGameInfo();
            Destroy(coll.gameObject);
        }
        else if (coll.gameObject.name.Contains("Item_Obj") == true)
        {
            GameMgr.Inst.InvenAddItem(coll.gameObject);
            Destroy(coll.gameObject);
        }

    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)
            return;

        GameMgr.Inst.DamageText((int)a_Value, transform.position);

        m_CurHp -= a_Value;

        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if (HpBarImg != null)
            HpBarImg.fillAmount = m_CurHp / m_MaxHp;

        if (m_CurHp <= 0.0f)
        {
            m_CurHp = 0.0f;

            //게임오버
        }
    }

    internal void ChangeNickName(string nickStr)
    {
        if(m_Nickname != null)
            m_Nickname.text = nickStr;
    }
}
