using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button BackBtn;

    //LeftGroup User List 관리 변수
    int g_UniqueUD = 0;
    [Header("LeftGroup UserList")]
    public ScrollRect m_LF_ScrollView;          //Scrollrect 컴포넌트가 붙어있는 오브젝트
    public GameObject m_LF_SvContent;           //ScrollContent 차일드로 생성될 parent 객체
    public GameObject m_LF_NodePrefab = null;   //Node Prefab

    public Button m_LF_AddNodeBtn = null;       //노드 추가 버튼
    public Button m_LF_SelDelBtn = null;        //노드 삭제 버튼
    public Button m_LF_MoveNodeBtn = null;      //찾는 노드로 이동 버튼
    public InputField m_LF_InputField = null;   //찾을 유니크ID 입력 필드
    //Content 하위의 차일드 목록을 찾기 위한 변수
    [HideInInspector] public LF_UserNode[] m_LF_UserNdList;


    //RightGroup Item List 관리 변수
    int g_Item_UniqueID = 0;
    [Header("RightGroup ItemList")]
    public ScrollRect m_RT_ScrollView;          //ScrollRect 컴포넌트가 붙어있는 오브젝트
    public GameObject m_RT_SvContent;           //ScrollContent 차일드로 생성될 Parent 객체
    public GameObject m_RT_NodePrefab = null;          //Node Prefab 

    public Button m_RT_AddNodeBtn = null;
    public Button m_RT_SelDelBtn = null;
    public Button m_RT_MoveNodeBtn = null;
    public InputField m_RT_InputField = null;

    [HideInInspector] public RT_ItemNode[] m_RT_ItemNdList;
    



    // Start is called before the first frame update
    void Start()
    {
        if (BackBtn != null)
            BackBtn.onClick.AddListener(BackBtnClick);

        //LeftGroup User List 관리 초기화 코드
        if (m_LF_AddNodeBtn != null)
            m_LF_AddNodeBtn.onClick.AddListener(LF_AddNodeClick);

        if (m_LF_SelDelBtn != null)
            m_LF_SelDelBtn.onClick.AddListener(LF_SelDelClick);

        if (m_LF_MoveNodeBtn != null)
            m_LF_MoveNodeBtn.onClick.AddListener(LF_MoveNodeClick);

        if (m_RT_AddNodeBtn != null)
            m_RT_AddNodeBtn.onClick.AddListener(RT_AddNodeClick);

        if (m_RT_SelDelBtn != null)
            m_RT_SelDelBtn.onClick.AddListener(RT_SelDelClick);

        if (m_RT_MoveNodeBtn != null)
            m_RT_MoveNodeBtn.onClick.AddListener(RT_MoveNodeClick);
    }

    private void RT_AddNodeClick()
    {
        if (m_RT_NodePrefab == null) return;

        GameObject a_ItemObj = Instantiate(m_RT_NodePrefab);
        a_ItemObj.transform.SetParent(m_RT_SvContent.transform, false);

        RT_ItemNode a_SvNode = a_ItemObj.GetComponent<RT_ItemNode>();
        int a_Rnd = Random.Range(0, 6);
        
        Item_Type a_Item_Type = (Item_Type)a_Rnd;

        string a_Name = a_Item_Type.ToString() + g_Item_UniqueID.ToString();
        
        int a_Level = g_Item_UniqueID;
        a_SvNode.InitInfo(g_Item_UniqueID, a_Item_Type, a_Name, a_Level);
        g_Item_UniqueID++;
    }

    private void RT_SelDelClick()
    {
        m_RT_ItemNdList = m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();

        int a_ItCount = m_RT_ItemNdList.Length;
        for(int i = 0; i < a_ItCount; i++)
        {
            if (m_RT_ItemNdList[i].m_SelectOnOff == true)
            {
                Destroy(m_RT_ItemNdList[i].gameObject);
            }
        }
    }

    private void RT_MoveNodeClick()
    {
        if (m_RT_InputField == null) return;

        string a_Str = m_RT_InputField.text.Trim();
        if(string.IsNullOrEmpty(a_Str) == true) return;

        int a_UniqueID = -1;
        if (int.TryParse(a_Str, out a_UniqueID) == false) return;

        m_RT_ItemNdList = m_RT_SvContent.transform.GetComponentsInChildren<RT_ItemNode>();
        int a_FIdx = -1;

        for (int i = 0;i < m_RT_ItemNdList.Length; i++) 
        { 
            if (a_UniqueID == m_RT_ItemNdList[i].m_UniqueID)
            {
                a_FIdx = m_RT_ItemNdList[i].transform.GetSiblingIndex();
                m_RT_ItemNdList[i].ItemSelect();    //찾은 아이템 선택까지 추가
                break;
            }
        }

        int a_NodeCount = m_RT_ItemNdList.Length;
        if (0 <= a_FIdx && a_FIdx < a_NodeCount)
        {
            if (0 < a_FIdx)
                a_FIdx = a_FIdx + 1;

            float normailizePosition = a_FIdx / (float)a_NodeCount;
            m_RT_ScrollView.verticalNormalizedPosition = 1.0f - normailizePosition;
        }
    }

    private void LF_AddNodeClick()
    {
        if (m_LF_NodePrefab == null) return;

        GameObject a_UserObj = Instantiate(m_LF_NodePrefab);
        a_UserObj.transform.SetParent(m_LF_SvContent.transform, false);

        LF_UserNode a_SvNode = a_UserObj.GetComponent<LF_UserNode>();
        string a_UName = "User_" + g_UniqueUD.ToString();
        int a_Level = Random.Range(2, 30);
        a_SvNode.InitInfo(g_UniqueUD, a_UName, a_Level);
        g_UniqueUD++;

    }

    private void LF_SelDelClick()
    {
        //LF_UserNode라는 스크립트를 가진 칠드런을 모두 찾아옴
        m_LF_UserNdList = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();

        int a_UsCount = m_LF_UserNdList.Length;
        for (int i = 0; i < a_UsCount; i++)
        {
            if (m_LF_UserNdList[i].m_SelectOnOff == true)
            {
                Destroy(m_LF_UserNdList[i].gameObject); //배열에 연결되어 있는 게임오브젝트를 삭제
            }
        }
    }

    private void LF_MoveNodeClick()
    {
        //입력상자 관련 예외처리
        if (m_LF_InputField == null) return;

        string a_GestStr = m_LF_InputField.text.Trim();
        if (string.IsNullOrEmpty(a_GestStr) == true) return;

        int a_UniqueID = -1;
        if(int.TryParse(a_GestStr, out a_UniqueID) == false) return;

        //셀 목록 배열로 가져오기
        m_LF_UserNdList = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();
        int a_FindIdx = -1; //만약 없는 유니크ID를 입력했다면 파인드인덱스 값이 -1이기 때문에 검색이 안됨

        //입력한 유니크ID랑 셀 목록 배열의 유니크ID를 대조
        for(int i = 0; i < m_LF_UserNdList.Length; i++)
        {
            if(a_UniqueID == m_LF_UserNdList[i].m_UniqueID)     //발견했다면
            {
                //차일드로 붙어 있는 순서 인덱스를 가져오는 함수
                a_FindIdx = m_LF_UserNdList[i].transform.GetSiblingIndex();
                break;
            }
        }

        //m_LF_UserNdList.Length : Content에 붙어있는 차일드 갯수 얻어오기
        //m_LF_SvContent.transform.childCount : Content에 붙어있는 차일드 갯수 얻어오기
        //m_LF_ScrollView.content.transform.childCount : Content에 붙어있는 차일드 갯수 얻어오기
        int a_NodeCount = m_LF_UserNdList.Length;   //전체 셀의 갯수
        if(0 <= a_FindIdx && a_FindIdx < a_NodeCount)
        {
            //마지막 노드로 이동시키려고 할 때 확실히 이동시키려는 코드
            //갯수는 1부터 시작, 인덱스는 0부터 시작이기 때문에
            if (0 < a_FindIdx)
                a_FindIdx = a_FindIdx + 1;

            //1.0일 때가 시작 위치, 0.0에 가까워질수록 끝 위치
            float normalizePosition = a_FindIdx / (float)a_NodeCount; //전체 갯수 대비 파인드인덱스의 위치를 비율로 저장
            m_LF_ScrollView.verticalNormalizedPosition = 1.0f - normalizePosition;  //1일때가 위이니 값을 반전시키기 위해 1에서 빼줌
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BackBtnClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}
