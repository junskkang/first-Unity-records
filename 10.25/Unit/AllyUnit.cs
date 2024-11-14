using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UnitState
{
    idle,
    wounded,
    die
}

public class Ally_Attribute //속성
{
    public AllyType type;
    public string unitName = "";
    public int level = 0;
    public int unlockCost = 0;
    public int buildCost = 0;

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
    [HideInInspector] public Ally_Attribute ally_Attribute = null;

    [HideInInspector] public int curLevel = 0;
    [HideInInspector] public float levelUpCost = 0;
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

    bool skillEnabled = false;

    [HideInInspector] public bool isSkilled = false;
    [HideInInspector] public bool isDoTHeal = false;
    [HideInInspector] public GameObject whosHeal = null;
    [HideInInspector] public bool isAccel = false;
    [HideInInspector] public float accel = 0.0f;
    [HideInInspector] public GameObject whosAccel = null;

    [HideInInspector] public Vector3 cacDir = Vector3.zero;
    
    [HideInInspector] public GameManager m_RefGameMgr = null;  //InGameMgr와 소통을 위한 객체

    //UI 관련 변수
    Canvas canvas = null;
    public bool isClicked = false;
    Image hpBar = null;
    //EventTrigger eventTrigger = null;
    SpriteRenderer mesh = null;
    RectTransform[] rangeUIs = new RectTransform[2];
    GameObject levelUpEff = null;
    GameObject MasterEff = null;

    //애니메이션 변화
    Animator anim = null;

    //유닛 상태
    UnitState unitState = UnitState.idle;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //캐릭터 공통 등장 준비
        m_RefGameMgr = FindObjectOfType<GameManager>(); //InGameManager와 소통을 위한 객체를 찾아 놓음
        levelUpEff = Resources.Load("LevelUpEff") as GameObject;
        MasterEff = Resources.Load("MasterEff") as GameObject;
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
        if (unitState == UnitState.die) return;

        if (!isSkilled) //&& MonsterGenerator.inst.curMonCount != 0
            curAttCool -= isAccel ? Time.deltaTime * accel : Time.deltaTime;

        if (curAttCool <= 0 && attackCount == skillPossible && !isSkilled && skillEnabled)
        {
            isSkilled = true;
            Skill();
            curAttCool = unitState == UnitState.wounded? curAttSpeed * 0.7f : curAttSpeed;
        }

        if (curAttCool <= 0 && !isSkilled)
        {
            Attack();
            curAttCool = unitState == UnitState.wounded ? curAttSpeed * 0.7f : curAttSpeed;
        }

        UIUpdate();

        AnimUpdate();
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
        //unitName = ally_Attribute.unitName;
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

        levelUpCost = ally_Attribute.buildCost;

        //Debug.Log("스탯 설정 완료");
    }

    void UISetUp()
    {
        //eventTrigger = GetComponent<EventTrigger>();

        //EventTrigger.Entry entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerDown;
        //entry.callback.AddListener((Data) =>
        //                            { 
        //                                OnPointerClick((PointerEventData)Data);
        //                            });
        //eventTrigger.triggers.Add(entry);
        
        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>(true);

        if (mesh == null)
            mesh = GetComponentInChildren<SpriteRenderer>();
        
        if (canvas != null)
            hpBar = canvas.transform.Find("Hpbar").GetComponent<Image>();

        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        if (rangeUIs != null)
        {
            string str = "";
            for (int i = 0; i < rangeUIs.Length; i++)
            {
                //0 == 어택, 1 == 스킬                
                if (i == 0)
                    str = "Attack";
                else if( i == 1)                    
                    str = "Skill";                
                
                rangeUIs[i] = canvas.transform.Find($"{str}Range").GetComponent<RectTransform>();
                
                if(i == 0)
                    rangeUIs[i].localScale = new Vector3(curAttRange * 2, curAttRange * 2, 1f);
                else if (i == 1)
                    rangeUIs[i].localScale = new Vector3(skillRange * 2, skillRange * 2, 1f);
            }            
        }        
    }

    void UIUpdate()
    {            

        if (hpBar != null)
            hpBar.fillAmount = curHp / maxHp;
    }

    public void Levelup()
    {        
        if (curLevel >= 10) return;
        //Debug.Log("유닛 레벨업" + ally_Attribute.unitName); 
        if (levelUpCost > GlobalValue.g_Gold) return;

        Debug.Log(levelUpCost);

        if (levelUpEff != null)
        {
            GameObject go = Instantiate(levelUpEff);
            go.transform.position = transform.position;
            go.GetComponentInChildren<SpriteRenderer>().sortingOrder = mesh.sortingOrder + 1;
            Destroy(go, 1.6f);
        }

        curLevel++;

        GameManager.Inst.GetGold(-(int)levelUpCost);

        curAttDamage += 1.0f;
        curAttRange += 0.1f;
        curAttSpeed -= 0.1f;

        skillRange += 0.2f;
        skillDamage += 2.0f;
        if (curLevel % 2 == 0)
        {
            float sizeUp = 1.0f + (float)curLevel / 20.0f;
            //Debug.Log(sizeUp);
            mesh.transform.localScale = new Vector3(sizeUp, sizeUp);
        }

        if (curLevel == 4)
        {
            skillEnabled = true;
            attackCount = 0;

            Debug.Log("스킬 사용 가능 여부 : " + skillEnabled);
        }
        else if (curLevel % 5 == 0)
        {
            skillPossible -= 1;
            skillHitLimit += 1;
        }

        if (curLevel == 10 && MasterEff != null)
        {
            GameObject go = Instantiate(MasterEff);
            go.transform.position = transform.position;
            go.GetComponentInChildren<SpriteRenderer>().sortingOrder = - 1;
            go.transform.SetParent(transform, true);
        }


        if (rangeUIs != null)
        {
            
            for (int i = 0; i < rangeUIs.Length; i++)
            {
                if (i == 0)
                    rangeUIs[i].localScale = new Vector3(curAttRange * 2, curAttRange * 2, 1f);
                else if (i == 1)
                    rangeUIs[i].localScale = new Vector3(skillRange * 2, skillRange * 2, 1f);
            }
        }

        levelUpCost = ally_Attribute.buildCost * (1 + curLevel); 
    }

    public void UnitClick()
    {
        isClicked = !isClicked;

        if (isClicked)
        {
            canvas.gameObject.SetActive(true);
            canvas.sortingOrder += 5;
            mesh.sortingOrder += 5;
        }
        else
        {
            canvas.gameObject.SetActive(false);
            canvas.sortingOrder -= 5;
            mesh.sortingOrder -= 5;
        }
    }

    void AnimUpdate()
    {
        if (curHp <= 0)
        {
            curHp = 0;
            unitState = UnitState.die;
            anim.Play($"Ally{((int)ally_Attribute.type + 1)}_Die");
        }
        else if (curHp < 30.0f)
        {
            unitState = UnitState.wounded;
            anim.Play($"Ally{((int)ally_Attribute.type + 1)}_Wounded");
        }
        else
        {
            unitState = UnitState.idle;
            anim.Play($"Ally{((int)ally_Attribute.type + 1)}_Idle");
        }
    }
}
