using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement; // Event Trigger�� �̿��ϱ� ���� �ʿ�

enum JoyStickType
{
    Fixed = 0,      //���̽�ƽ ����
    Flexible,       //Ŭ���� ��ġ�� ���̽�ƽ �̵�   
    FlexibleOnOff   //���̽�ƽ �Ⱥ��̴ٰ� Ŭ���� ��ġ�� ���̽�ƽ �����
}

public class GameMgr : MonoBehaviour
{
    //�ҷ��������� ���ΰ��� ���͵� ����� �������� �ѹ��� �ε���Ű���� ��
    //���� �������� ����ϴµ� ���ΰ��� ���Ϳ��� ���� �ε���Ű�� ���ʿ��� ����
    public static GameObject m_BulletPrefab = null;

    public Button m_BackBtn;

    [Header("User Info UI")]
    //�����̺� �Ӽ��ε� ����Ƽ �ν����� â�� �߰��ϴ� �ɼ� [SerializeField]
    //�ٸ� Ŭ���������� �� ������ �������� ���ϵ��� ������ �ν����� â���� ���� �����ϰԲ�
    //[HideInInspector] : public �Ӽ��̶� �ٸ� Ŭ�������� ���� ���������� ����Ƽ �ν����� â���� �������Բ�
    [SerializeField] private Button m_UserInfoBtn = null;


    //Fixed JoyStick ó��
    JoyStickType m_JoyStickType = JoyStickType.Fixed;   //ó�� ������ Fixed 

    [Header("JoyStick")]
    public GameObject m_JoySBackObj = null;
    public Image m_JoyStickImg = null;
    float m_Radius = 0.0f;
    Vector3 m_OriginPos = Vector3.zero;     //���̽�ƽ�� ����
    Vector3 m_Axis = Vector3.zero;          //����
    Vector3 m_JsCacVec = Vector3.zero;      
    float m_JsCacDist = 0.0f;               //���� �巡���� �Ÿ�

    //Flexible JoyStick
    public GameObject m_JoyStickPickPanel = null;
    Vector3 posJoyBack;
    Vector3 dirStick;

    //������ ����� ����
    Vector3 m_StCacPos = Vector3.zero;
    [Header("Damage Text")]
    public Transform m_Damage_Canvas = null;
    public GameObject m_DamageTxtRoot = null;

    [HideInInspector] public HeroCtrl m_RefHero = null;


    //�̱��� ���� ����
    public static GameMgr Inst;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;   //���� ������ 60���� ����
        QualitySettings.vSyncCount = 0;

        m_RefHero = FindObjectOfType<HeroCtrl>();
        //���ӸŴ����� �����ϸ鼭 �ҷ��������� �ε���
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
            //v[0] : �����ϴ�, v[1] : �������, v[2] : �������, v[3] : �����ϴ�
            //�����ϴ��� (0, 0)�� UI ��ũ�� ��ǥ (Screen.width, Screen.height)��
            //�������� 
            m_Radius = v[2].y - v[0].y;
            m_Radius = m_Radius / 3.0f;     //���̽�ƽ�� ���� ������ ����� ���ϵ��� �ϴ� ������

            m_OriginPos = m_JoyStickImg.transform.position;

            EventTrigger trigger = m_JoySBackObj.GetComponent<EventTrigger>();
            //trigger��� ������ m_JoySBackObj�� �پ��ִ� EventTrigger ������Ʈ�� ������
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;  //�巡�װ� �߻����� ��
            entry.callback.AddListener((data) =>
            {
                OnDragJoyStick((PointerEventData)data);
            });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;   //�巡�װ� ������ ��
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
    private void OnDragJoyStick(PointerEventData a_data)    //�巡�� �ϴ� ���콺 ��ǥ�� �Ű�������
    {
        if (m_JoyStickImg == null)
            return;

        m_JsCacVec = (Vector3)a_data.position - m_OriginPos;
        m_JsCacVec.z = 0.0f;
        m_JsCacDist = m_JsCacVec.magnitude;
        m_Axis = m_JsCacVec.normalized;

        //���̽�ƽ ��׶��带 ����� ���ϰ� ���� �κ�
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
        
        //ĳ���� �̵� ó��
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);   //��� �������� ��ŭ�� ������
    }

    private void OnEndDragJoyStick(PointerEventData data)
    {
        if (m_JoyStickImg == null)
            return;

        m_Axis = Vector3.zero;      //���� �ʱ�ȭ
        m_JoyStickImg.transform.position = m_OriginPos;     //�������� �ʱ�ȭ
        m_JsCacDist = 0.0f;         //�Ÿ� �ʱ�ȭ

        //ĳ���� �̵� ����
        if (m_RefHero != null)
            m_RefHero.SetJoyStickMv(0.0f, Vector3.zero);
    }
    #endregion

    #region --- Flexible Joystick
    private void OnPointerDown_Flx(PointerEventData data)   //���콺 Ŭ����
    {
        if (data.button != PointerEventData.InputButton.Left)
            return; //�����Ϳ��� ���콺 ���� ��ư Ŭ���� �ƴϸ� ����

        if (m_JoySBackObj == null)
            return;

        if (m_JoyStickImg == null)
            return;

        //���콺 Ŭ���� ��ġ���� ���̽�ƽ�� �����ϰԲ�
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

        //ĳ���� ���� ó��
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

        //���̽�ƽ �̵�
        posJoyBack = m_JoySBackObj.transform.position;  //���̽�ƽ ���� ��ġ
        dirStick = data.position - (Vector2)posJoyBack; //����
        dirStick.z = 0.0f;
        m_JsCacDist = dirStick.magnitude;
        m_Axis = dirStick.normalized;

        if (m_Radius < m_JsCacDist)
        {
            m_JsCacDist = m_Radius;
            m_JoyStickImg.transform.position =
                posJoyBack + (m_Axis * m_Radius);  //������ ��ġ���� ��������ŭ
        }
        else
        {
            m_JoyStickImg.transform.position = data.position;
        }

        if (1.0f < m_JsCacDist)
            m_JsCacDist = 1.0f;

        //ĳ���� �̵�
        if(m_RefHero != null)
            m_RefHero.SetJoyStickMv(m_JsCacDist, m_Axis);
    }
    #endregion

    public void DamageText(int a_Value, Vector3 a_OwnerPos)
    {
        GameObject a_DmgClone = Instantiate(m_DamageTxtRoot);   //������ ����
        if (a_DmgClone != null && m_Damage_Canvas != null)
        {
            Vector3 a_StCacPos = new Vector3(a_OwnerPos.x, 0.8f, a_OwnerPos.z + 4.0f) ;     //��ġ ����ֱ�
            a_DmgClone.transform.SetParent(m_Damage_Canvas);    //������ �������� ĵ������ �ڽ�����
            DamageText a_DamageTx = a_DmgClone.GetComponent<DamageText>();  //��ũ��Ʈ �ٿ��ֱ�
            a_DamageTx.DamageTxtSpawn(a_Value, new Color32(200, 0, 0, 255));    //�Լ� ����
            a_DmgClone.transform.position = a_StCacPos; //��ġ�� �־��ֱ�
        }
    }
    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
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
