using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Event Trigger를 이용하기 위해 필요

enum JoyStickType
{
    Fixed = 0,      //조이스틱 고정
    Flexible,       //클릭한 위치로 조이스틱 이동   
    FlexibleOnOff   //조이스틱 안보이다가 클릭한 위치에 조이스틱 생기는
}

public class GameMgr : MonoBehaviour
{
    //불렛프리팹을 주인공도 몬스터도 사용할 목적으로 한번만 로딩시키려고 함
    //같은 프리팹을 사용하는데 주인공과 몬스터에서 각각 로딩시키면 불필요한 낭비
    public static GameObject m_BulletPrefab = null;

    public Button m_BackBtn;

    [Header("User Info UI")]
    //프라이빗 속성인데 유니티 인스펙터 창에 뜨게하는 옵션 [SerializeField]
    //다른 클래스에서는 이 변수에 접근하지 못하도록 하지만 인스펙터 창에는 연결 가능하게끔
    //[HideInInspector] : public 속성이라 다른 클래스에서 접근 가능하지만 유니티 인스펙터 창에서 가려지게끔
    [SerializeField] private Button m_UserInfoBtn = null;


    //Fixed JoyStick 처리
    JoyStickType m_JoyStickType = JoyStickType.Fixed;   //처음 시작은 Fixed 

    [Header("JoyStick")]
    public GameObject m_JoySBackObj = null;
    public Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OriginPos = Vector3.zero;     //조이스틱의 중점
    Vector3 m_Axis = Vector3.zero;          //방향
    Vector3 m_JsCacVec = Vector3.zero;      
    float m_JsCacDist = 0.0f;               //내가 드래그한 거리

    //Flexible JoyStick
    public GameObject m_JoyStickPickPanel = null;
    Vector3 posJoyBack;
    Vector3 dirStick;

    //데미지 띄우기용 변수
    Vector3 m_StCacPos = Vector3.zero;
    [Header("Damage Text")]
    public Transform m_Damage_Canvas = null;
    public GameObject m_DamageTxtRoot = null;

    [HideInInspector] public HeroCtrl m_RefHero = null;


    //싱글턴 패턴 접근
    public static GameMgr Inst;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;   //실행 프레임 60으로 고정
        QualitySettings.vSyncCount = 0;

        m_RefHero = FindObjectOfType<HeroCtrl>();
        //게임매니저는 시작하면서 불렛프리팹을 로딩함
        m_BulletPrefab = Resources.Load("BulletPrefab") as GameObject;

        if (m_BackBtn != null)
            m_BackBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        #region --- Fixed Joystick 
        if (m_JoySBackObj != null && m_JoyStickImg != null &&
            m_JoySBackObj.activeSelf == true
            && m_JoyStickPickPanel.activeSelf == false)
        {
            m_JoyStickType = JoyStickType.Fixed;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            //v[0] : 좌측하단, v[1] : 좌측상단, v[2] : 우측상단, v[3] : 우측하단
            //좌측하단이 (0, 0)인 UI 스크린 좌표 (Screen.width, Screen.height)를
            //기준으로 
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;     //조이스틱이 일정 영역을 벗어나지 못하도록 하는 반지름

            m_OriginPos = m_JoyStickImg.transform.position;

            EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
            //trigger라는 변수에 m_JoySBackObj에 붙어있는 EventTrigger 컴포넌트를 가져옴
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;  //드래그가 발생했을 때
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;   //드래그가 끝났을 때
            entry.callback.AddListener((data) =>
            {
                OnEndDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);
        }
        #endregion
        #region --- Flexible Joystick

        if (m_JoyStickPickPanel != null && m_JoySBackObj != null
            && m_JoyStickImg != null && m_JoyStickPickPanel.activeSelf == true)
        {
            if (m_JoySBackObj.activeSelf == true)
                m_JoyStickType = JoyStickType.Flexible;
            else
                m_JoyStickType = JoyStickType.FlexibleOnOff;

            Vector3[] v = new Vector3[4];
            m_JoySBackObj.GetComponent<RectTransform>().GetWorldCorners(v);
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;

            m_OriginPos = m_JoyStickImg.transform.position;
            m_JoySBackObj.GetComponent<Image>().raycastTarget = false;
            m_JoyStickImg.raycastTarget = false;

            EventTrigger trigger = m_JoyStickPickPanel.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) =>
            {
                OnPointerDown_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener((data) =>
            {
                OnPointerUp_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry) ;

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick_Flx((PointerEventData)data);
            });
            trigger.triggers.Add(entry) ;
        }
        #endregion
    }


    // Update is called once per frame
    void Update()
    {
        
    }

#region --- Fixed Joystick 
    private void OnDragJoyStick(PointerEventData a_data)    //드래그 하는 마우스 좌표를 매개변수로
    {
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = (Vector3)a_data.position - m_OriginPos;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        //조이스틱 백그라운드를 벗어나지 못하게 막는 부분
        if (m_Radius < m_JsCacDist)
        {
            m_JoyStickImg.transform.position = m_OriginPos + m_Axis * m_Radius;
        }
        else
        {
            m_JoyStickImg.transform.position = m_OriginPos + m_Axis * m_JsCacDist;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;
        
        //캐릭터 이동 처리
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);   //어느 방향으로 얼만큼의 힘으로
    }

    private void OnEndDragJoyStick(PointerEventData data)
    {
        if (m_JoyStickImg == null)
            return;

        m_Axis = Vector3.zero;      //방향 초기화
        m_JoyStickImg.transform.position = m_OriginPos;     //중점으로 초기화
        m_JsCacDist = 0.0f;         //거리 초기화

        //캐릭터 이동 정지
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }
    #endregion

    #region --- Flexible Joystick
    private void OnPointerDown_Flx(PointerEventData data)   //마우스 클릭시
    {
        if (data.button != PointerEventData.InputButton.Left)
            return; //에디터에서 마우스 왼쪽 버튼 클릭이 아니면 리턴

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        //마우스 클릭한 위치에서 조이스틱이 시작하게끔
        m_JoySBackObj.transform.position = data.position;
        m_JoyStickImg.transform.position = data.position;

        m_JoySBackObj.SetActive(true);
    }

    private void OnPointerUp_Flx(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        m_JoySBackObj.transform.position = m_OriginPos;
        m_JoyStickImg.transform.position = m_OriginPos;

        if (m_JoyStickType == JoyStickType.FlexibleOnOff)
        {
            m_JoySBackObj.SetActive(false);

        }

        m_Axis = Vector3.zero;
        m_JsCacDist = 0.0f;

        //캐릭터 정지 처리
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }

    private void OnDragJoyStick_Flx(PointerEventData data)
    {
        if (data.button != PointerEventData.InputButton.Left)
            return;

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        //조이스틱 이동
        posJoyBack = m_JoySBackObj.transform.position;  //조이스틱 시작 위치
        dirStick = data.position - (Vector2)posJoyBack; //방향
        dirStick.z = 0.0f;
        m_JsCacDist = dirStick.magnitude;
        m_Axis = dirStick.normalized;

        if (m_Radius < m_JsCacDist)
        {
            m_JsCacDist = m_Radius;
            m_JoyStickImg.transform.position =
                posJoyBack + (m_Axis * m_Radius);  //시작한 위치에서 반지름만큼
        }
        else
        {
            m_JoyStickImg.transform.position = data.position;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;

        //캐릭터 이동
        if(m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);
    }
    #endregion

    public void DamageText(int a_Value, Vector3 a_OwnerPos)
    {
        GameObject a_DmgClone = Instantiate(m_DamageTxtRoot);   //프리팹 생성
        if (a_DmgClone != null && m_Damage_Canvas != null)
        {
            Vector3 a_StCacPos = new Vector3(a_OwnerPos.x, 0.8f, a_OwnerPos.z + 4.0f) ;     //위치 잡아주기
            a_DmgClone.transform.SetParent(m_Damage_Canvas);    //생성된 프리팹을 캔버스의 자식으로
            DamageText a_DamageTx = a_DmgClone.GetComponent<DamageText>();  //스크립트 붙여주기
            a_DamageTx.DamageTxtSpawn(a_Value, new Color32(200, 0, 0, 255));    //함수 실행
            a_DmgClone.transform.position = a_StCacPos; //위치값 넣어주기
        }
    }
    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 
}
