using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Store_Mgr : MonoBehaviour
{
    public Button BackBtn;
    public Text m_UserInfoText = null;

    public GameObject m_Item_ScContent; //ScrollView Content 차일드로 생성될 Parent 객체
    public GameObject m_SkProductNode;  //Node Prefab

    SkProductNode[] m_SkNodeList;       //스크롤에 붙어 있는 Item 목록들...

    //--- 지금 뭘 구입하려고 시도한 건지? 저장해 놓기 위한 변수
    SkillType m_BuySkType;  //어떤 스킬 아이템을 구입하려고 한 건지?
    int m_SvMyGold;         //구입 프로세스에 진입 후 상태 저장용 : 차감된 내 골드가 얼마인지?
    int m_SvMyCount = 0;    //스킬 보유수 증가 백업해 놓기...
    //--- 지금 뭘 구입하려고 시도한 건지? 저장해 놓기 위한 변수

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameData();

        if (BackBtn != null)
            BackBtn.onClick.AddListener(BackBtnClick);

        if (m_UserInfoText != null)
            m_UserInfoText.text = "별명(" + GlobalValue.g_NickName + ") : 보유골드(" +
                                    GlobalValue.g_UserGold + ")";

        //--- 아이템 목록 추가
        GameObject a_ItemObj = null;
        SkProductNode a_SkItemNode = null;
        for(int i = 0; i < GlobalValue.g_SkDataList.Count; i++)
        {
            a_ItemObj = Instantiate(m_SkProductNode);
            a_SkItemNode = a_ItemObj.GetComponent<SkProductNode>();
            a_SkItemNode.InitData(GlobalValue.g_SkDataList[i].m_SkType);
            a_ItemObj.transform.SetParent(m_Item_ScContent.transform, false);
        }
        //--- 아이템 목록 추가

        RefreshSkItemList();

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        
    }

    void BackBtnClick()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    void RefreshSkItemList()
    {
        if(m_Item_ScContent != null)
        {
            if (m_SkNodeList == null || m_SkNodeList.Length <= 0)
                m_SkNodeList = m_Item_ScContent.GetComponentsInChildren<SkProductNode>();
        }

        for(int i = 0; i < m_SkNodeList.Length; i++)
        {
            m_SkNodeList[i].RefreshState();
        }
    }//void RefreshSkItemList()
    public void BuySkillItem(SkillType a_SkType)
    {  //리스트 뷰에 있는 캐릭터 가격 버튼을 눌러 구입 시도를 한 경우
        m_BuySkType = a_SkType;     //구매하려는 아이 지역변수로 저장
        BuyBeforeJobCo();
    }

    void BuyBeforeJobCo()   //구매 1단계 함수
    {
        //서버로부터 골드, 아이템 상태 받아와서 클라이언트와 동기화 시켜주기

        if (GlobalValue.g_Unique_ID == "") return;  //로그인 되어있는 상태에서만 동작하도록


        //플레이어 데이터(타이틀) 값 요청하기
        var request = new GetUserDataRequest()
        {
            PlayFabId = GlobalValue.g_Unique_ID
        };

        PlayFabClientAPI.GetUserData(
            request,
            (result) =>
            {
                //유저 정보 받아오기 성공 했을 때
                PlayerDataParse(result);
            },
            (error) =>
            {
                //유저 정보 받아오기 실패 했을 때
            }
            );
    }

    void PlayerDataParse(GetUserDataResult result)
    {
        bool isParseFail = false; //parse failed  //서버의 정보를 변환 실패
        bool a_IsDifferent = false; //서버의 값과 갖고있는 값이 다른 경우

        int getValue = 0;
        int Idx = 0;
        //서버에 존재하는 값과 로컬에 존재하는 값을 비교하는 것
        foreach (var eachData in result.Data)  //아래의 케이스는 받아온 데이터에 이상이 있는 경우
        {
            if (eachData.Key == "UserGold") 
            {
                //혹시나 int가 아닌 다른 데이터형의 값으로 장난질이 들어올 경우를 대비
                //정상적이라면 이미 앞에 if문에서 Key를 UserGold로 거르고 들어왔기 때문에
                //해당 if에 걸릴 일은 없음
                if (int.TryParse(eachData.Value.Value, out getValue) == false)  
                {
                    isParseFail = true;
                    continue;
                }                    
                if (getValue != GlobalValue.g_UserGold) 
                {
                    a_IsDifferent = true;
                    break;
                }

                //GlobalValue.g_UserGold = getValue;
            }
            else if (eachData.Key.Contains("Skill_Item_") == true)
            {
                Idx = 0;
                string[] strArr = eachData.Key.Split('_');      // _를 기준으로 나누어 저장
                if (3 <= strArr.Length)         //Skill_Item_1 이런식으로 되어있기에 3덩어리로 나올것
                {
                    //2번째 인덱스에 있는 것은 아이템 넘버인데 트라이파세가 실패했다? 비정상          
                    if (int.TryParse(strArr[2], out Idx) == false)
                    {
                        isParseFail = true;
                        continue;
                    }
                }
                else
                {
                    isParseFail = true;
                    continue;
                }

                if (GlobalValue.g_CurSkillCount.Count <= Idx)
                {
                    isParseFail = true;
                    continue;
                }

                if (int.TryParse(eachData.Value.Value, out getValue) == false)
                {
                    isParseFail = true;
                    continue;
                }

                if ((int)m_BuySkType != Idx) //지금 구매 하려고 하는 상품만 다른지 확인한다.
                    continue;

                if (getValue != GlobalValue.g_CurSkillCount[Idx])
                {
                    a_IsDifferent = true;
                    break;
                }

                //GlobalValue.g_CurSkillCount[Idx] = getValue;
            }
        }

        string a_Mess = "";
        bool a_NeedDelegate = false;
        Skill_Info a_SkInfo = GlobalValue.g_SkDataList[(int)m_BuySkType];

        if (a_IsDifferent)
            a_Mess = "서버의 골드와 스킬 아이템 정보가 정상적이지 않습니다.\n운영진에 문의해 주세요.";
        else if (5 <= GlobalValue.g_CurSkillCount[(int)m_BuySkType])
        {
            a_Mess = "하나의 아이템은 5개까지만 구매할 수 있습니다.";
        }
        else if(GlobalValue.g_UserGold < a_SkInfo.m_Price)
        {
            a_Mess = "보유(누적) 골드가 부족합니다.";
        }
        else
        {
            a_Mess = "정말 구입하시겠습니까?";
            a_NeedDelegate = true;      //<-- 이 조건일 때 구매
        }

        if (isParseFail)
        {
            a_Mess += "\n(서버에 비정상 정보가 있습니다.)";
        }

        
        //m_BuySkType = a_SkType;

        //구매를 했을 경우에 적용시켜줄 용도로 백업해놓는 변수
        m_SvMyGold = GlobalValue.g_UserGold;
        m_SvMyGold -= a_SkInfo.m_Price; //해당 아이템 구매 성공시 갖게 될 골드값 미리 저장
        m_SvMyCount = GlobalValue.g_CurSkillCount[(int)m_BuySkType];
        m_SvMyCount++;  //스킬 보유수 증가 백업해 놓기

        GameObject a_DlgRsc = Resources.Load("DialogBox") as GameObject;
        GameObject a_DlgBoxObj = Instantiate(a_DlgRsc);
        GameObject a_Canvas = GameObject.Find("Canvas");
        a_DlgBoxObj.transform.SetParent(a_Canvas.transform, false);
        DialogBox_Ctrl a_DlgBox = a_DlgBoxObj.GetComponent<DialogBox_Ctrl>();
        if(a_DlgBox != null)
        {
            if (a_NeedDelegate == true)
                a_DlgBox.InitMessage(a_Mess, TryBuySkItem);
            else
                a_DlgBox.InitMessage(a_Mess);
        }

    }//public void BuySkillItem(SkillType a_SkType)

    void TryBuySkItem() //구매 2단계 확정 함수 (서버에 데이터 값 전달하기)
    {
        if (GlobalValue.g_Unique_ID == "") return;  //로그인 되어 있을 경우만 동작하도록

        Dictionary<string, string> itemList = new Dictionary<string, string>();
        itemList.Add("UserGold", m_SvMyGold.ToString());
        itemList.Add($"Skill_Item_{(int)m_BuySkType}", m_SvMyCount.ToString());

        var request = new UpdateUserDataRequest()
        {
            Data = itemList
        };
        PlayFabClientAPI.UpdateUserData(request,
                        (result) =>
                        {
                            //메뉴 상태 갱신
                            GlobalValue.g_UserGold = m_SvMyGold;    //골드값 조정
                            GlobalValue.g_CurSkillCount[(int)m_BuySkType] = m_SvMyCount;    //스킬 보유수 증가 조정

                            RefreshSkItemList();

                            m_UserInfoText.text = "별명(" + GlobalValue.g_NickName +
                                                    ") : 보유골드(" + GlobalValue.g_UserGold + ")";
                        },
                        (error) =>
                        {
                            Debug.Log("데이터 저장 실패");
                        }
                        );
    }
    //void TryBuySkItem()  //구매 3단계 확정 함수
    //{
    //    if (m_BuySkType < SkillType.Skill_0 || SkillType.SkCount <= m_BuySkType)
    //        return;

    //    GlobalValue.g_UserGold = m_SvMyGold;    //골드값 조정
    //    GlobalValue.g_CurSkillCount[(int)m_BuySkType] = m_SvMyCount;    //스킬 보유수 증가 조정

    //    RefreshSkItemList();

    //    m_UserInfoText.text = "별명(" + GlobalValue.g_NickName +
    //                            ") : 보유골드(" + GlobalValue.g_UserGold + ")";

        ////--- 로컬에 저장
        //PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);
        //PlayerPrefs.SetInt($"Skill_Item_{(int)m_BuySkType}", 
        //                                GlobalValue.g_CurSkillCount[(int)m_BuySkType]);
        ////--- 로컬에 저장
    //}
}
