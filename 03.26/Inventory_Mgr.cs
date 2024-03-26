using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class Inventory_Mgr : MonoBehaviour
{
    //유저 정보
    public static string g_UserName = "SBS힐러";
    public static int g_GameGold = 1000;
        
    List<CItem> m_ItemList = new List<CItem>();

    [Header("--- AddEditNode ---")]
    public GameObject AddNodeRoot = null;
    public InputField ItName_Ifd = null;

    public Button Add_Btn = null;
    public Button Find_Btn = null;
    public Text PrintList = null;

    public Button ShowList_Btn = null;
    public Button LvSort_Btn = null;
    public Button GdSort_Btn = null;
    public Button Clear_Btn = null;

    public Text Help_Text = null;
    public Text UserInfo_Text = null;

    [Header("--- UpLvMode ---")]
    public GameObject UpLvModeRoot = null;
    public Button UpLvModeRootClose_Btn = null;
    public Button Sell_Btn = null;
    public Button LvUp_Btn = null;
    public Button GdUp_Btn = null;
    public Text UpLvMdFindInfo = null;

    public Text m_LvRateText = null;
    public Text m_GdRateText = null;

    float m_ShowTime = 0.0f;
    int m_FIndex = -1;
    // Start is called before the first frame update
    void Start()
    {
        LoadGameData();
        ReFreshUIList(m_ItemList);

        if (Add_Btn != null)
            Add_Btn.onClick.AddListener(AddBtnClick);       //아이템 추가

        if (Clear_Btn != null)
            Clear_Btn.onClick.AddListener(ClearBtnClick);   //전체 노드 삭제

        if (ShowList_Btn != null)
            ShowList_Btn.onClick.AddListener(ShowBtnClick);

        if (LvSort_Btn != null)
            LvSort_Btn.onClick.AddListener(LvSortBtnClick);

        if (GdSort_Btn != null)
            GdSort_Btn.onClick.AddListener(GdSortBtnClick);

        if (Find_Btn != null)
            Find_Btn.onClick.AddListener(FindBtnClick);

        if (Sell_Btn != null)
            Sell_Btn.onClick.AddListener(SellBtnClick);

        if (UpLvModeRootClose_Btn != null)
            UpLvModeRootClose_Btn.onClick.AddListener(() =>
            {
                m_FIndex = -1;
                UpLvMdFindInfo.text = "";
                ItName_Ifd.text = "";

                if (UpLvModeRoot != null)
                    UpLvModeRoot.SetActive(false);
            });
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_ShowTime)     //타이머 3초 뒤에 삭제
        {
            m_ShowTime -= Time.deltaTime;
            if(m_ShowTime <= 0.0f)
            {
                if(Help_Text != null)
                    Help_Text.gameObject.SetActive(false);
            }
        }
    }

    void AddBtnClick()
    {
        if (ItName_Ifd == null)
            return;

        string a_ItName = ItName_Ifd.text.Trim();

        if(string.IsNullOrEmpty(a_ItName) == true)      //입력상자에 아이템 이름 입력 안할경우 자동생성
        { 
            CItem a_Item = new CItem();
            a_Item.InitItem();
            m_ItemList.Add(a_Item);
        }
        else
        {
            m_ItemList.Add (new CItem(a_ItName));       //입력상자에 아이템 이름 입력 할 경우
        }

        SaveGameData();
        ReFreshUIList(m_ItemList);   //화면 갱신
        ItName_Ifd.text = "";
    }

    void ShowBtnClick()      //리스트를 추가순으로 보여주는 정렬
    {
        ReFreshUIList(m_ItemList);
    }

    void LvSortBtnClick()
    {
        List<CItem> a_CopyList = m_ItemList.ToList();
        a_CopyList.Sort((a, b) => b.m_Level.CompareTo(a.m_Level));  //레벨 내림차순
        ReFreshUIList (a_CopyList);
    }

    void GdSortBtnClick()
    {
        List<CItem> a_CopyList = m_ItemList.ToList();
        a_CopyList.Sort((a, b) => a.m_Grade.CompareTo(b.m_Grade));  //등급 오름차순
        ReFreshUIList (a_CopyList);
    }
    void ClearBtnClick()
    {
        m_ItemList.Clear();
        PlayerPrefs.DeleteAll();
        LoadGameData();
        ReFreshUIList(m_ItemList);
        ItName_Ifd.text = "";
    }

    void FindBtnClick()
    {
        if(ItName_Ifd == null)
            return;

        string a_strItemId = ItName_Ifd.text.Trim();
        ItName_Ifd.text = "";

        if (string.IsNullOrEmpty(a_strItemId) == true)
            return;

        int a_CIdx = -1;
        if (int.TryParse(a_strItemId, out a_CIdx) == false)
            return;

        m_FIndex = m_ItemList.FindIndex(x => x.m_ItemUId == a_CIdx);

        if (m_FIndex < 0)   //입력한 ItemId는 존재하지 않는다는 뜻
        {
            if(Help_Text != null)
            {
                Help_Text.gameObject.SetActive(true);
                Help_Text.text = "찾는 아이템 Id는 존재하지 않습니다.";
                m_ShowTime = 3.0f;
            }

            return;
        }

        if(UpLvModeRoot != null)
        {
            UpLvModeRoot.SetActive(true);
        }

        //검색 아이템 정보 출력
        CItem a_FNode = m_ItemList[m_FIndex];
        string a_strBuff = $"[<color = #ff99ff>({a_FNode.m_ItemUId})</color> {a_FNode.m_Name}] : " +
                         $"레벨({a_FNode.m_Level}) 등급({a_FNode.m_Grade}) 가격({a_FNode.m_Price})";

        UpLvMdFindInfo.text = a_strBuff;


    }

    void SellBtnClick()
    {
        if (m_FIndex < 0 || m_ItemList.Count <= m_FIndex)
            return;
        
        int a_CacCost = (int)(m_ItemList[m_FIndex].m_Price * 0.8f);
        g_GameGold += a_CacCost;
        Help_Text.gameObject.SetActive(true);
        Help_Text.text = "유저 보유 골드 + " + a_CacCost + " 상승!!";
        m_ShowTime = 3.0f;
        
        //아이템 인덱스 삭제
        m_ItemList.RemoveAt(m_FIndex);

        //각종 수치들 초기화
        m_FIndex = -1;
        UpLvMdFindInfo.text = "";
        ItName_Ifd.text = "";
        SaveGameData();
        ReFreshUIList(m_ItemList);

        if (UpLvModeRoot != null)
            UpLvModeRoot.SetActive(false);
    }
    void SaveGameData()
    {
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt("CurUniqId", CItem.g_CurUniqId);
        PlayerPrefs.SetString("NickName", g_UserName);
        PlayerPrefs.SetInt("GameGold", g_GameGold);

        if (m_ItemList.Count <= 0)
            return;

        PlayerPrefs.SetInt("It_Count", m_ItemList.Count);    //아이템 수 저장

        CItem a_Node;
        for(int i = 0; i < m_ItemList.Count; i++)
        {
            a_Node = m_ItemList[i];
            PlayerPrefs.SetInt($"IT_{i}_ItemUId", a_Node.m_ItemUId);
            PlayerPrefs.SetString($"IT_{i}_Name", a_Node.m_Name);
            PlayerPrefs.SetInt($"IT_{i}_Level", a_Node.m_Level);
            PlayerPrefs.SetInt($"IT_{i}_Grade", a_Node.m_Grade);
            PlayerPrefs.SetInt($"IT_{i}_Price", a_Node.m_Price);
        }
    }
    void LoadGameData()
    {
        CItem.g_CurUniqId = PlayerPrefs.GetInt("CurUniqId", 0);
        g_UserName = PlayerPrefs.GetString("NickName", "SBS힐러");
        g_GameGold = PlayerPrefs.GetInt("GameGold", 1000);

        int a_ItCount = PlayerPrefs.GetInt("It_Count", 0); //저장되어있는 아이템 리스트 갯수 불러옴

        if (a_ItCount <= 0)
            return;

        CItem a_Node;
        for(int i=0; i < a_ItCount; i++)  //아이템 리스트 갯수만큼 불러옴
        {
            a_Node = new CItem();

            a_Node.m_ItemUId = PlayerPrefs.GetInt($"IT_{i}_ItemUId", -1);
            a_Node.m_Name = PlayerPrefs.GetString($"IT_{i}_Name", "");
            a_Node.m_Level = PlayerPrefs.GetInt($"IT_{i}_Level", 0);
            a_Node.m_Grade = PlayerPrefs.GetInt($"IT_{i}_Grade", 0);
            a_Node.m_Price = PlayerPrefs.GetInt($"IT_{i}_Price", 0);
            
            if (CItem.g_CurUniqId <= a_Node.m_ItemUId)
            {
                Debug.Log($"ItemID_{a_Node.m_ItemUId}번 {a_Node.m_Name} 아이템은 중복될 수 있는 번호이므로 삭제되었습니다.");
                continue;  //중복된 Id가 나올 수 있는 상황을 피하기 위한 코드
            }

            m_ItemList.Add(a_Node);
        }
    }

    void ReFreshUIList(List<CItem> a_ItemList = null)
    {
        UserInfo_Text.text = $"[{g_UserName}] : Gold({g_GameGold})";

        if (a_ItemList == null)
            return;

        PrintList.text = "";

        if (a_ItemList.Count <= 0)
            return;

        string a_StrBuff = "";
        for(int i = 0; i< a_ItemList.Count; i++)
        {
            a_StrBuff += $"[<color = #ff99ff>({a_ItemList[i].m_ItemUId})</color> {a_ItemList[i].m_Name}] : " +
                         $"레벨({a_ItemList[i].m_Level}) 등급({a_ItemList[i].m_Grade}) 가격({a_ItemList[i].m_Price})";

            a_StrBuff += "\n"; //줄바꿈 기호
        }

        PrintList.text = a_StrBuff;
    }
}
