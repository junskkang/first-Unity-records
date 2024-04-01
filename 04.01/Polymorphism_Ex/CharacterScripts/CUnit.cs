using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharicType
{
    Wizard,
    Barbarian,
    Archer,
    Count
}


public class Chr_Stat      //Stat, Atrribute 설정데이터
{
    public CharicType m_CharType;
    public string m_StrJob = "";
    public string m_Name = "";
    public string m_RscFile = "";          //리소스 프리팹 이름
    public int m_MaxHp = 0;
    public int m_MaxMana = 0;
    public int m_Attack = 0;

    //캐릭터의 스텟 정보보를 기반으로 지형에 돌아다니는 캐릭터 객체를 추가하려고 할 때
    //Hero 게임 오브젝트에 빙의 시키길 원하는 클래스 추가 함수
    public virtual CUnit MyAddComponent(GameObject a_Hero)
    {
        CUnit a_RefHero = null;
        return a_RefHero;
    }
}

public class CUnit : MonoBehaviour
{
    [HideInInspector] public Chr_Stat m_ChrStat = null;

    [HideInInspector] public int m_CurHp = 0;     //게임 중에 변하는 Hp
    [HideInInspector] public int m_CurMana = 0;   //게임 중에 변하는 Mana
    [HideInInspector] public int m_CurAtt = 0;    //게임 중에 변하는 공격력

    [HideInInspector] public InGameManager m_RefGameMgr = null;  //InGameMgr와 소통을 위한 객체

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //캐릭터 공통 등장 준비
        m_RefGameMgr = FindObjectOfType<InGameManager>(); //InGameManager와 소통을 위한 객체를 찾아 놓음

        //캐릭터 등장시 호출함수 (외형 모델링 로딩 리소스 셋팅, 캐릭터 고유 이펙트 메모리풀 준비)
        //상속받는 쪽 공통 등장 코드(스폰 위치 등)
        
        //리소스 로딩
        GameObject a_ChrSprite = Resources.Load(m_ChrStat.m_RscFile)as GameObject;
        if (a_ChrSprite != null)
        {
            GameObject a_Clone = Instantiate(a_ChrSprite); //지금 이 Hero GameObject에 붙일 스프라이트 스폰
            a_Clone.transform.SetParent(this.transform, false); //Hero 밑에 child로 스프라이트 붙이기
        }
        //스탯치 상태 변수에 충전
        m_CurHp = m_ChrStat.m_MaxHp;
        m_CurMana = m_ChrStat.m_MaxMana;
        m_CurAtt = m_ChrStat.m_Attack;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //캐릭터의 공통 행동패턴 동작처리
    }

    public virtual void Attack()
    {
        //캐릭터의 공통 공격 동작처리
    }

    public virtual void UseSkill()
    {
        //캐릭터의 공통 스킬 동작처리
    }
}
