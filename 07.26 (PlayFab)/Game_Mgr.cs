using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    GameIng,            //���� ���� ���� ����
    GameEnd,            //���ӿ��� ����
    GameExit,           //�κ�� ���� �������� ��
    GameReplay          //���ӿ������� �ٽ� ���� ������ ���
}

public class Game_Mgr : MonoBehaviour
{
    public GameState State = GameState.GameIng;     //���� ���� �� ���·� ����

    public Text m_BestScoreText = null; //�ִ����� ǥ�� UI
    public Text m_CurScoreText = null;  //�������� ǥ�� UI
    public Text m_GoldText = null;      //������� ǥ�� UI
    public Text m_UserInfoText = null;  //���� ���� ǥ�� UI
    public Button GoLobbyBtn = null;    //�κ�� �̵� ��ư

    int m_CurScore = 0;     //�̹� ������������ ���� ���� ����
    int m_CurGold = 0;      //�̹� ������������ ���� ��尪

    //--- ĳ���� �Ӹ����� ������ ����� ���� ����
    GameObject m_DmgClone;  //Damage Text ���纻�� ���� ����
    DmgTxt_Ctrl m_DmgTxt;   //Damage Text ���纻�� �پ� �ִ� DmgTxt_Ctrl ������Ʈ�� ���� ����
    Vector3 m_StCacPos;     //���� ��ġ�� ����� �ֱ� ���� ����
    [Header("--- Damage Text ---")]
    public Transform Damage_Canvas = null;
    public GameObject DmgTxtRoot = null;
    //--- ĳ���� �Ӹ����� ������ ����� ���� ����

    //--- ���� ������ ���� ����
    GameObject m_CoinItem = null;
    //--- ���� ������ ���� ����

    //--- ��Ʈ ������ ���� ����
    GameObject m_HeartItem = null;
    //--- ��Ʈ ������ ���� ����

    [Header("--- Skill Coll Timer ---")]
    public Transform m_SkillCoolRoot = null;
    public GameObject m_SkCollNode = null;

    HeroCtrl m_RefHero = null;

    [Header("--- Inventory Show OnOff ---")]
    public Button m_Inven_Btn = null;
    public Transform m_InventoryRoot = null;
    Transform m_ArrowIcon = null;
    bool m_Inven_ScOnOff = true;
    float m_ScSpeed = 1500.0f;
    Vector3 m_ScOnPos = new Vector3(-170.0f, 0.0f, 0.0f);
    Vector3 m_ScOffPos = new Vector3(-572.0f, 0.0f, 0.0f);

    //--- Config Box(ȯ�漳��) ���� ����
    [Header("------- ConfigBox -------")]
    public Button m_CfgBtn = null;
    public GameObject Canvas_Dialog = null;
    GameObject m_ConfigBoxObj = null;
    //--- Config Box(ȯ�漳��) ���� ����

    [Header("------- Game Over -------")]
    public GameObject GameOverPanel = null;
    public Text Result_Text = null;
    public Button Replay_Btn = null;
    public Button RstLobby_Btn = null;

    [Header("------- ExpLevel -------")]
    public Text exp_Text;
    public Text lv_Text;
    public Image expImg;
    //int monKillcount = 0;

    //�κ񰡱� ����� ����
    public GameObject LoadingPanel;
    public Image loadingCharacter;

    //--- �̱��� ����
    public static Game_Mgr Inst = null;

    void Awake()
    {
        Inst = this;
    }
    //--- �̱��� ����

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //���� �ӵ���...
        State = GameState.GameIng;
        GlobalValue.LoadGameData();
        InitRefreshUI();
        RefreshSkillList();
        RefreshExpLevelUI();

        if (GoLobbyBtn != null)
            GoLobbyBtn.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("LobbyScene");  
                //��Ŷó���� ������ ��Ű�� ������ ���� ��� �ӹ�������
                //��ٸ��� ���� �߰� �ʿ�
                Time.timeScale = 0.0f;
                LoadingPanel.gameObject.SetActive(true);
                StartCoroutine(Loading());
               
                //State = GameState.GameExit;
            });

        m_CoinItem = Resources.Load("CoinPrefab") as GameObject;
        m_HeartItem = Resources.Load("HeartPrefab") as GameObject;

        m_RefHero = GameObject.FindObjectOfType<HeroCtrl>();

        if (m_Inven_Btn != null)
        {
            m_ArrowIcon = m_Inven_Btn.transform.Find("ArrowIcon");

            m_Inven_Btn.onClick.AddListener(() =>
            {
                m_Inven_ScOnOff = !m_Inven_ScOnOff;
            });
        }

        //--- ȯ�漳�� Dlg ���� ���� �ڵ�
        if (m_CfgBtn != null)
            m_CfgBtn.onClick.AddListener(() =>
            {
                if (m_ConfigBoxObj == null)
                    m_ConfigBoxObj = Resources.Load("ConfigBox") as GameObject;

                GameObject a_CfgBoxObj = Instantiate(m_ConfigBoxObj) as GameObject;
                a_CfgBoxObj.transform.SetParent(Canvas_Dialog.transform, false);
                a_CfgBoxObj.GetComponent<ConfigBox>().DltMethod = CfgResponse;

                Time.timeScale = 0.0f;  //�Ͻ�����
            });
        //--- ȯ�漳�� Dlg ���� ���� �ڵ�

        Sound_Mgr.Inst.PlayBGM("sound_bgm_island_001", 0.5f);

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        //--- ����Ű �̿����� ��ų ����ϱ�
        if(Input.GetKeyDown(KeyCode.Alpha1) ||
           Input.GetKeyDown(KeyCode.Keypad1))
        {
            UseSkill_Key(SkillType.Skill_0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2) ||
            Input.GetKeyDown(KeyCode.Keypad2))
        {
            UseSkill_Key(SkillType.Skill_1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) ||
            Input.GetKeyDown(KeyCode.Keypad3))
        {
            UseSkill_Key(SkillType.Skill_2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) ||
                Input.GetKeyDown(KeyCode.Keypad4))
        {
            UseSkill_Key(SkillType.Skill_3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) ||
                Input.GetKeyDown(KeyCode.Keypad5))
        {
            UseSkill_Key(SkillType.Skill_4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) ||
                Input.GetKeyDown(KeyCode.Keypad6))
        {
            UseSkill_Key(SkillType.Skill_5);
        }

        ScrollOnOff_Update();

    }//void Update()

    public void DamageText(float a_Value, Vector3 a_Pos, Color a_Color)
    {
        if(Damage_Canvas == null || DmgTxtRoot == null) 
            return;

        m_DmgClone = Instantiate(DmgTxtRoot);
        m_DmgClone.transform.SetParent(Damage_Canvas);
        m_DmgTxt = m_DmgClone.GetComponent<DmgTxt_Ctrl>();
        if (m_DmgTxt != null)
            m_DmgTxt.InitDamage(a_Value, a_Color);
        m_StCacPos = new Vector3(a_Pos.x, a_Pos.y + 1.14f, 0.0f);
        m_DmgClone.transform.position = m_StCacPos;
    }

    public void SpawnCoin(Vector3 a_Pos, int a_Value = 10)
    {
        if(m_CoinItem == null)
            return;

        GameObject a_CoinObj = Instantiate(m_CoinItem);
        a_CoinObj.transform.position = a_Pos;
        Coin_Ctrl a_CoinCtrl = a_CoinObj.GetComponent<Coin_Ctrl>();
        if (a_CoinCtrl != null)
            a_CoinCtrl.m_RefHero = m_RefHero;
    }

    public void SpawnHeart(Vector3 a_Pos)
    {
        if (m_HeartItem == null)
            return;

        GameObject a_HeartObj = Instantiate(m_HeartItem);
        a_Pos.z = 0.0f;
        a_HeartObj.transform.position = a_Pos;
    }

    void UseSkill_Key(SkillType a_SkType)
    {
        if (GlobalValue.g_CurSkillCount[(int)a_SkType] <= 0)
            return;     //�����ϰ� �ִ� ��ų �������� ����� �� ����

        if(m_RefHero == null)
            return;

        m_RefHero.UseSkill(a_SkType);

        //--- ��ų ������ ���� UI ���� �ڵ�
        if (m_InventoryRoot == null)
            return;

        SkInvenNode[] a_SkIvnNodes = m_InventoryRoot.GetComponentsInChildren<SkInvenNode>();
        for (int i = 0; i < a_SkIvnNodes.Length; i++)
        {
            if (a_SkIvnNodes[i].m_SkType == a_SkType)
            {
                a_SkIvnNodes[i].Refresh_UI(a_SkType);
                break;
            }
        }
        //--- ��ų ������ ���� UI ���� �ڵ�
    }

    public void SkillCoolMethod(SkillType a_SkType, float a_Time, float a_During)
    {
        GameObject a_Obj = Instantiate(m_SkCollNode);
        a_Obj.transform.SetParent(m_SkillCoolRoot, false);
        //�ι�° �Ű����� worldPositionStays (�⺻���� true)

        SkillCool_Ctrl a_SCtrl = a_Obj.GetComponent<SkillCool_Ctrl>();  
        if(a_SCtrl != null)
            a_SCtrl.InitState(a_SkType, a_Time, a_During);
    }

    void ScrollOnOff_Update()
    {
        if(m_InventoryRoot == null) 
            return;

        if(Input.GetKeyDown(KeyCode.R) == true)
        {
            m_Inven_ScOnOff = !m_Inven_ScOnOff;
        }

        if(m_Inven_ScOnOff == false)
        {
            if(m_InventoryRoot.localPosition.x > m_ScOffPos.x)
            {
                m_InventoryRoot.localPosition =
                    Vector3.MoveTowards(m_InventoryRoot.localPosition, 
                                            m_ScOffPos, m_ScSpeed * Time.deltaTime);

                if(m_ScOffPos.x <= m_InventoryRoot.localPosition.x)
                {
                    m_ArrowIcon.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
                }
            }//if(m_InventoryRoot.localPosition.x > m_ScOffPos.x)
        }// if(m_Inven_ScOnOff == false)
        else //if(m_Inven_ScOnOff == true)
        {
            if(m_ScOnPos.x > m_InventoryRoot.localPosition.x)
            {
                m_InventoryRoot.localPosition =
                    Vector3.MoveTowards(m_InventoryRoot.localPosition,
                                            m_ScOnPos, m_ScSpeed * Time.deltaTime);

                if(m_InventoryRoot.localPosition.x <= m_ScOnPos.x)
                {
                    m_ArrowIcon.transform.eulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
                }
            }
        }//else //if(m_Inven_ScOnOff == true)

    }//void ScrollOnOff_Update()

    public void AddScore(int a_Value = 10)
    {
        if (m_CurScore <= int.MaxValue - a_Value)
            m_CurScore += a_Value;
        else
            m_CurScore = int.MaxValue;

        if (m_CurScore < 0)
            m_CurScore = 0;

        m_CurScoreText.text = "��������(" + m_CurScore + ")";
        if(GlobalValue.g_BestScore < m_CurScore)
        {
            GlobalValue.g_BestScore = m_CurScore;
            m_BestScoreText.text = "�ְ�����(" + GlobalValue.g_BestScore + ")";
            //PlayerPrefs.SetInt("BestScore", GlobalValue.g_BestScore);
            Network_Mgr.instance.PushPacket(PacketType.BestScore);
        }
    }

    public void AddGold(int a_Value = 10)
    {
        //�̹� ������������ ���� ��尪
        if(m_CurGold <= int.MaxValue - a_Value)
            m_CurGold += a_Value;
        else
            m_CurGold = int.MaxValue;

        //���ÿ� ����Ǿ� �ִ� ���� ���� ��尪
        if(GlobalValue.g_UserGold <= int.MaxValue - a_Value)
            GlobalValue.g_UserGold += a_Value;
        else
            GlobalValue.g_UserGold = int.MaxValue;

        m_GoldText.text = "�������(" + GlobalValue.g_UserGold + ")";
        //PlayerPrefs.SetInt("UserGold", GlobalValue.g_UserGold);

        //��Ʈ��ũ�� ���� ��Ŷ�� ����Ʈ�� ����
        Network_Mgr.instance.PushPacket(PacketType.UserGold);
    }

    public void AddExpLevel(int addExp = 50)
    {        
        if (GlobalValue.g_Level < GlobalValue.g_LvTable.Count)  //���� �ְ� ������ �������� �ʾ��� ��
        {
            GlobalValue.g_Exp += addExp;   //����ġ ȹ��

            //����ġ�� �ʿ����ġ �̻����� ���� ��츦 ����
            for (int i = 0; i < 100; i++)  //�ִ����� ���� �ִ뷹���� �־�θ� ��
            {
                //������ ����ġ�� ������Ų ����� ���������� ������Ű�� ���� ����ġ�� ���� �������� �ʾҴٸ�..
                if (GlobalValue.g_Exp < GlobalValue.g_LvTable[GlobalValue.g_Level].destExp)
                    break;  //������ ������Ű�� �ʵ��� �Ʒ��� �������� �ڵ� ��ŵ
                //���� GlobalValue.g_Exp�� ����ġ�� �����Ǵ� ���� ��

                //���� ����ġ�� �ʿ����ġ�� �Ѿ�� ���� ������ ����
                GlobalValue.g_Exp -= GlobalValue.g_LvTable[GlobalValue.g_Level].destExp;
                GlobalValue.g_Level++;

                if (GlobalValue.g_LvTable.Count <= GlobalValue.g_Level) //�ְ����� ����
                    break;
            }
                    
        }

        Network_Mgr.instance.PushPacket(PacketType.UpdateExp);

        RefreshExpLevelUI();
    }

    void RefreshExpLevelUI()
    {
        //UI ����

        string a_strExp = "<color=#ff0000>�ְ���������</color>";
        if (GlobalValue.g_Level < GlobalValue.g_LvTable.Count)
        {
            a_strExp = $"����ġ ({GlobalValue.g_Exp} / {GlobalValue.g_LvTable[GlobalValue.g_Level].destExp})";
        }

        if (exp_Text != null)
            exp_Text.text = a_strExp + $" ����({GlobalValue.g_Level + 1})";

        if (lv_Text != null)
            lv_Text.text = $"Lv.{GlobalValue.g_Level + 1}";

        if (expImg != null)      
            expImg.fillAmount = ((float)GlobalValue.g_Exp) / ((float)GlobalValue.g_LvTable[GlobalValue.g_Level].destExp);
    }
    void InitRefreshUI()
    {
        if (m_BestScoreText != null)
            m_BestScoreText.text = "�ְ�����(" + GlobalValue.g_BestScore + ")";

        if (m_CurScoreText != null)
            m_CurScoreText.text = "��������(" + m_CurScore + ")";

        if (m_GoldText != null)
            m_GoldText.text = "�������(" + GlobalValue.g_UserGold + ")";

        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ")";
    }

    void RefreshSkillList() //���� Skill Item ����� UI�� �����ϴ� �Լ�
    {
        SkInvenNode[] a_SkIvnNodes = m_InventoryRoot.GetComponentsInChildren<SkInvenNode>();
        for(int i = 0; i < a_SkIvnNodes.Length; i++)
        {
            if (GlobalValue.g_CurSkillCount.Count <= i)
                continue;

            a_SkIvnNodes[i].InitState((SkillType)i);

        }//for(int i = 0; i < a_SkIvnNodes.Length; i++)

    }//void RefreshSkillList()

    public void GameOverMethod()
    {
        if (GameOverPanel != null && GameOverPanel.activeSelf == false)
            GameOverPanel.SetActive(true);

        if (Result_Text != null)
            Result_Text.text = "NickName\n" + GlobalValue.g_NickName + "\n\n" +
                                "ȹ������\n" + m_CurScore + "\n\n" +
                                "ȹ����\n" + m_CurGold;

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("GameScene");
                State = GameState.GameReplay;
            });

        if (RstLobby_Btn != null)
            RstLobby_Btn.onClick.AddListener(() =>
            {
                //SceneManager.LoadScene("LobbyScene");
                State = GameState.GameExit;
            });
    }

    void CfgResponse() //ȯ�漳�� �ڽ� Ok �� ȣ��ǰ� �ϱ� ���� �Լ�
    {       
        if (m_UserInfoText != null)
            m_UserInfoText.text = "������ : ����(" + GlobalValue.g_NickName + ")";
    }

    IEnumerator Loading()
    {
        yield return new WaitForSecondsRealtime(0.7f);

        if (loadingCharacter != null)
            loadingCharacter.transform.position = new Vector3(loadingCharacter.transform.position.x + 95.0f, loadingCharacter.transform.position.y);

        yield return new WaitForSecondsRealtime(0.7f);

        if (loadingCharacter != null)
            loadingCharacter.transform.position = new Vector3(loadingCharacter.transform.position.x + 95.0f, loadingCharacter.transform.position.y);

        yield return new WaitForSecondsRealtime(0.7f);

        State = GameState.GameExit;
    }

}//public class Game_Mgr : MonoBehaviour