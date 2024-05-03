using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button BackBtn;

    //LeftGroup User List ���� ����
    int g_UniqueUD = 0;
    [Header("LeftGroup UserList")]
    public ScrollRect m_LF_ScrollView;          //Scrollrect ������Ʈ�� �پ��ִ� ������Ʈ
    public GameObject m_LF_SvContent;           //ScrollContent ���ϵ�� ������ parent ��ü
    public GameObject m_LF_NodePrefab = null;   //Node Prefab

    public Button m_LF_AddNodeBtn = null;       //��� �߰� ��ư
    public Button m_LF_SelDelBtn = null;        //��� ���� ��ư
    public Button m_LF_MoveNodeBtn = null;      //ã�� ���� �̵� ��ư
    public InputField m_LF_InputField = null;   //ã�� ����ũID �Է� �ʵ�
    //Content ������ ���ϵ� ����� ã�� ���� ����
    [HideInInspector] public LF_UserNode[] m_LF_UserNdList;


    //RightGroup Item List ���� ����
    int g_Item_UniqueID = 0;
    [Header("RightGroup ItemList")]
    public ScrollRect m_RT_ScrollView;          //ScrollRect ������Ʈ�� �پ��ִ� ������Ʈ
    public GameObject m_RT_SvContent;           //ScrollContent ���ϵ�� ������ Parent ��ü
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

        //LeftGroup User List ���� �ʱ�ȭ �ڵ�
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
                m_RT_ItemNdList[i].ItemSelect();    //ã�� ������ ���ñ��� �߰�
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
        //LF_UserNode��� ��ũ��Ʈ�� ���� ĥ�己�� ��� ã�ƿ�
        m_LF_UserNdList = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();

        int a_UsCount = m_LF_UserNdList.Length;
        for (int i = 0; i < a_UsCount; i++)
        {
            if (m_LF_UserNdList[i].m_SelectOnOff == true)
            {
                Destroy(m_LF_UserNdList[i].gameObject); //�迭�� ����Ǿ� �ִ� ���ӿ�����Ʈ�� ����
            }
        }
    }

    private void LF_MoveNodeClick()
    {
        //�Է»��� ���� ����ó��
        if (m_LF_InputField == null) return;

        string a_GestStr = m_LF_InputField.text.Trim();
        if (string.IsNullOrEmpty(a_GestStr) == true) return;

        int a_UniqueID = -1;
        if(int.TryParse(a_GestStr, out a_UniqueID) == false) return;

        //�� ��� �迭�� ��������
        m_LF_UserNdList = m_LF_SvContent.transform.GetComponentsInChildren<LF_UserNode>();
        int a_FindIdx = -1; //���� ���� ����ũID�� �Է��ߴٸ� ���ε��ε��� ���� -1�̱� ������ �˻��� �ȵ�

        //�Է��� ����ũID�� �� ��� �迭�� ����ũID�� ����
        for(int i = 0; i < m_LF_UserNdList.Length; i++)
        {
            if(a_UniqueID == m_LF_UserNdList[i].m_UniqueID)     //�߰��ߴٸ�
            {
                //���ϵ�� �پ� �ִ� ���� �ε����� �������� �Լ�
                a_FindIdx = m_LF_UserNdList[i].transform.GetSiblingIndex();
                break;
            }
        }

        //m_LF_UserNdList.Length : Content�� �پ��ִ� ���ϵ� ���� ������
        //m_LF_SvContent.transform.childCount : Content�� �پ��ִ� ���ϵ� ���� ������
        //m_LF_ScrollView.content.transform.childCount : Content�� �پ��ִ� ���ϵ� ���� ������
        int a_NodeCount = m_LF_UserNdList.Length;   //��ü ���� ����
        if(0 <= a_FindIdx && a_FindIdx < a_NodeCount)
        {
            //������ ���� �̵���Ű���� �� �� Ȯ���� �̵���Ű���� �ڵ�
            //������ 1���� ����, �ε����� 0���� �����̱� ������
            if (0 < a_FindIdx)
                a_FindIdx = a_FindIdx + 1;

            //1.0�� ���� ���� ��ġ, 0.0�� ����������� �� ��ġ
            float normalizePosition = a_FindIdx / (float)a_NodeCount; //��ü ���� ��� ���ε��ε����� ��ġ�� ������ ����
            m_LF_ScrollView.verticalNormalizedPosition = 1.0f - normalizePosition;  //1�϶��� ���̴� ���� ������Ű�� ���� 1���� ����
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
