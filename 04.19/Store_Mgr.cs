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

    [HideInInspector] public LF_UserNode[] m_LF_UserNdLiad;
    //Content ������ ���ϵ� ����� ã�� ���� ����


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
        
    }

    private void LF_MoveNodeClick()
    {
        
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
