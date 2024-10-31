using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;


public class Ally_Atrribute //속성
{
    public AllyType type;
    public string name = "";
    public int level = 0;

    public float maxHp = 0;
    public float maxMp = 0;
    public float attackDamage = 0;
    public float attackRange = 0;
    public float attackSpeed = 0;
    public float attackCool = 0;

    public int attackCount = 0;
    public int skillPossible = 0;
    public bool anyHit = false;
    public float skillRange = 0;
    public float skillDamage = 0;
    public int skillHitLimit = 0;

    public GameObject attackEff;
    public GameObject skillEff;
    
    //캐릭터의 스텟 정보보를 기반으로 지형에 돌아다니는 캐릭터 객체를 추가하려고 할 때
    //Hero 게임 오브젝트에 빙의 시키길 원하는 클래스 추가 함수
    public virtual AllyUnit MyAddComponent(GameObject parentObject)
    {
        AllyUnit refAlly = null;
        return refAlly;
    }  
}

public class AllyUnit : MonoBehaviour
{
    [HideInInspector] public Ally_Atrribute ally_Attribute = null;

    [HideInInspector] public int curLevel = 0;

    [HideInInspector] public float maxHp = 0;     //게임 중에 변하는 Hp
    public float curHp = 0;     //게임 중에 변하는 Hp
    [HideInInspector] public float curMp = 0;   //게임 중에 변하는 Mana
    [HideInInspector] public float curAttDamage = 0;    //게임 중에 변하는 공격력
    [HideInInspector] public float curAttRange = 0;
    [HideInInspector] public float curAttSpeed = 0;
    [HideInInspector] public float curAttCool = 0;

    [HideInInspector] public int attackCount = 0;
    [HideInInspector] public int skillPossible = 0;
    [HideInInspector] public bool anyHit = false;
    [HideInInspector] public float skillRange = 0;
    [HideInInspector] public float skillDamage = 0;
    [HideInInspector] public int skillHitLimit = 0;
    public int monKill = 0;

    [HideInInspector] public bool isSkilled = false;

    [HideInInspector] public Vector3 cacDir = Vector3.zero;
    
    [HideInInspector] public GameManager m_RefGameMgr = null;  //InGameMgr와 소통을 위한 객체

    public Canvas canvas = null;
    public Image hpBar = null;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        //캐릭터 공통 등장 준비
        m_RefGameMgr = FindObjectOfType<GameManager>(); //InGameManager와 소통을 위한 객체를 찾아 놓음

        //캐릭터 등장시 호출함수 (외형 모델링 로딩 리소스 셋팅, 캐릭터 고유 이펙트 메모리풀 준비)
        //상속받는 쪽 공통 등장 코드(스폰 위치 등)

        //리소스 로딩 >> 프리팹으로 변경
        //GameObject a_ChrSprite = Resources.Load(Ally_Atrribute.m_RscFile) as GameObject;
        //if (a_ChrSprite != null)
        //{
        //    GameObject a_Clone = Instantiate(a_ChrSprite); //지금 이 Hero GameObject에 붙일 스프라이트 스폰
        //    a_Clone.transform.SetParent(this.transform, false); //Hero 밑에 child로 스프라이트 붙이기
        //}
        
        
        //스탯치 상태 변수에 충전       
        StatSetUp();

        //UI연결
        UISetUp();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        curAttCool -= Time.deltaTime;
        if (curAttCool <= 0 && attackCount == skillPossible && !isSkilled)
        {
            isSkilled = true;
            Skill();
            curAttCool = curAttSpeed;
        }

        if (curAttCool <= 0 && !isSkilled)
        {
            Attack();
            curAttCool = curAttSpeed;

            curHp -= curAttDamage / 2;
        }

        UIUpdate();
    }

    public virtual void Attack()
    {
        //캐릭터의 공통 공격 동작처리
    }

    public virtual void Skill()
    {
        //캐릭터의 공통 스킬 동작처리
    }

    void StatSetUp()
    {
        curLevel = ally_Attribute.level;
        maxHp = ally_Attribute.maxHp;
        curHp = maxHp;
        curMp = ally_Attribute.maxMp;

        curAttDamage = ally_Attribute.attackDamage;
        curAttRange = ally_Attribute.attackRange;
        curAttSpeed = ally_Attribute.attackSpeed;
        curAttCool = curAttSpeed;

        attackCount = ally_Attribute.attackCount;
        skillPossible = ally_Attribute.skillPossible;
        anyHit = ally_Attribute.anyHit;
        skillRange = ally_Attribute.skillRange;
        skillDamage = ally_Attribute.skillDamage;
        skillHitLimit = ally_Attribute.skillHitLimit;

        Debug.Log("스탯 설정 완료");
    }

    void UISetUp()
    {
        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>();

        if (canvas != null)
            hpBar = canvas.transform.Find("Hpbar").GetComponent<Image>();       
    }

    void UIUpdate()
    {
        if (hpBar != null)
            hpBar.fillAmount = curHp / maxHp;
    }

    public void Levelup()
    {
        if (curLevel >= 10) return;
        curLevel++;

        curAttDamage += 1.0f;
        curAttRange += 0.1f;
        curAttSpeed -= 0.1f;

        skillRange += 0.2f;
        skillDamage += 2.0f;

        if (curLevel % 4 == 0)
        {
            skillPossible -= 1;
            skillHitLimit += 1;
        }
    }
}
