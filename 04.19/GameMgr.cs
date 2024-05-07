using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Threading;// Event Trigger를 이용하기 위해 필요

//전체검색 키 : Ctrl + ,

//Terrain, NaviMesh << X,Z 평면에서 제공되는 유니티 기능
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
    [SerializeField] private RawImage userInfoPanel;
    bool m_InfoOnOff = false;
    public Text hpText;
    public Text bombCountText;
    public Text monKillCountText;
    public Text goldText;

    [SerializeField] public int monKillCount = 0;



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


    //인벤토리 스크롤뷰
    [Header("Inventory ScrollView")]
    public Button m_InvenBtn = null;
    public Transform m_InvenScrollTr = null;
    bool m_Inven_ScOnOff = false;
    float m_ScSpeed = 1800.0f;      //인벤토리 열리는 속도 조절 함수
    Vector3 m_ScOnPos = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 m_ScOffPos = new Vector3(320.0f, 0.0f, 0.0f);

    public Transform m_MkInvenContent = null;
    public GameObject m_MkItemNode = null;
    public Button m_ItemSellBtn = null;


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

        Time.timeScale = 1.0f;
        GlobalUserData.LoadGameInfo();
        RefreshInGameItemScv();


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

        //인벤토리 판넬 On Off
        if (m_InvenBtn != null)
            m_InvenBtn.onClick.AddListener(() =>
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
                if (m_ItemSellBtn != null)
                    m_ItemSellBtn.gameObject.SetActive(m_Inven_ScOnOff);
            });

        if (m_ItemSellBtn != null)
            m_ItemSellBtn.onClick.AddListener(ItemSelMethod);

        if (m_UserInfoBtn != null)
            m_UserInfoBtn.onClick.AddListener(() =>
            {
                m_InfoOnOff = !m_InfoOnOff;
                if (userInfoPanel != null)
                    userInfoPanel.gameObject.SetActive(m_InfoOnOff);
            });

    }


    // Update is called once per frame
    void Update()
    {
        InvenScOnOffUpdate();

        UserInfo();
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

    void InvenScOnOffUpdate()       //인벤토리 판넬 OnOff 연출 함수
    {
        if (m_InvenScrollTr == null) return;

        if (m_Inven_ScOnOff == false)
        {
            if (m_InvenScrollTr.localPosition.x < m_ScOffPos.x)
            {
                m_InvenScrollTr.localPosition =
                    Vector3.MoveTowards(m_InvenScrollTr.localPosition, m_ScOffPos, m_ScSpeed * Time.deltaTime);
            }
        }
        else //(m_Inven_ScOnOff == true)
        {
            if (m_ScOnPos.x < m_InvenScrollTr.localPosition.x)
            {
                m_InvenScrollTr.localPosition =
                    Vector3.MoveTowards(m_InvenScrollTr.localPosition, m_ScOnPos, m_ScSpeed * Time.deltaTime);
            }
        }

    }

    public void InvenAddItem(GameObject a_Obj)
    {
        ItemObjInfo a_RefItemInfo = a_Obj.GetComponent<ItemObjInfo>(); 
        if (a_RefItemInfo != null)
        {
            ItemValue a_Node = new ItemValue();
            a_Node.UniqueID = a_RefItemInfo.m_ItemValue.UniqueID;
            a_Node.m_Item_Type = a_RefItemInfo.m_ItemValue.m_Item_Type;
            a_Node.m_ItemName = a_RefItemInfo.m_ItemValue.m_ItemName;
            a_Node.m_ItemLevel = a_RefItemInfo.m_ItemValue.m_ItemLevel;
            a_Node.m_ItemStar = a_RefItemInfo.m_ItemValue.m_ItemStar;
            GlobalUserData.g_ItemList.Add(a_Node);
            
            AddNodeScrollView(a_Node);  //스크롤 뷰에 추가
            GlobalUserData.RefreshItemSave();   //파일 저장
        }
    }

    void AddNodeScrollView(ItemValue a_Node)
    {
        GameObject a_ItemObj = Instantiate(m_MkItemNode);
        a_ItemObj.transform.SetParent(m_MkInvenContent, false);
        //false일경우 : 로컬 기준의 정보를 유지한 채 차일드화 된다.

        //획득한 아이템에 따라 이미지 교체
        ItemNode a_MyItemInfo = a_ItemObj.GetComponent<ItemNode>();
        if (a_MyItemInfo != null)
            a_MyItemInfo.SetItemRsc(a_Node);
    }

    private void RefreshInGameItemScv() //인게임 스크롤뷰 갱신
    {
        //GlobalUserData.g_ItemList에 저장된 값을 scroll View에 복원해 주는 함수
        ItemNode[] a_MyNodeList =
            m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        for(int i = 0;  i < a_MyNodeList.Length; i++)
        {
            Destroy(a_MyNodeList[i]);
        }

        for (int i = 0; i < GlobalUserData.g_ItemList.Count; i++)
        {
            AddNodeScrollView(GlobalUserData.g_ItemList[i]);
        }
    }

    private void ItemSelMethod()
    {
        //아이템 하나당 100원씩 판매

        //스크롤뷰의 노드를 모두 돌면서 선택되어 있는 것들만 판매하고
        //해당 UniqueID를 g_ItemList에서 찾아서 제거해준다.
        ItemNode[] a_MyNodeList = 
            m_MkInvenContent.GetComponentsInChildren<ItemNode>(true);
        //true : Active가 꺼져 있는 오브젝트까지 모두 가져오는 옵션
        for (int i = 0; i < a_MyNodeList.Length; i++)
        {
            if (a_MyNodeList[i].m_SelOnOff == false)    //선택이 안되어 있는 애는 패스
                continue;

            //글로벌 리스트에서 판매하려는 아이템의 고유번호를 찾아서 제거해줘야 함.
            for(int b = 0; b< GlobalUserData.g_ItemList.Count; b++)
            {
                if (a_MyNodeList[i].m_UniqueID == GlobalUserData.g_ItemList[b].UniqueID)
                {
                    GlobalUserData.g_ItemList.RemoveAt(b);
                    break;
                }
            }

            Destroy(a_MyNodeList[i].gameObject);

            AddGold(100);
        }

        GlobalUserData.RefreshItemSave();   //변화가 생겼으니 다시 저장

    }

    public void UserInfo()
    {
        if (userInfoPanel == null)
            return;

        if (hpText != null)
            hpText.text = $"HP : {m_RefHero.m_CurHp} / {m_RefHero.m_MaxHp}";

        if (bombCountText != null)
            bombCountText.text = $"x  {GlobalUserData.g_BombCount}";

        if (monKillCountText != null)
            monKillCountText.text = $"x  {monKillCount}";

        if (goldText != null)
            goldText.text = $"x  {GlobalUserData.g_UserGold.ToString("N0")}";
    }

    public void AddGold(int a_Gold)
    {
        GlobalUserData.g_UserGold += a_Gold;

        GlobalUserData.SaveGameInfo();
    }

}
