using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready = 0,     //�÷��̾� ��߼����� �غ� ��
    Move = 1,      //�ڵ����� �����̰� �ִ� ����
    GameEnd = 2    //�÷��̾� 3���� �÷��̰� ��� ���� ����
}

public class PlayerData
{
    public int m_Index = 0;          //�÷��̾� ��ȣ
    public float m_SvLength = 0.0f;  //�÷��̾ ��� ����
    public int m_Ranking = -1;       //��ŷ ���� -1�� ���� ������ �Ű����� �ʾҴٴ� �ǹ�
}

public class GameDirector : MonoBehaviour
{
    public static GameState s_State = GameState.Ready;  //��𼭳� ���� ������ �� �ְ� static����

    GameObject car;
    GameObject flag;
    GameObject distance;

    public Text[] Player;  //�÷��̾ ��� Text UI
    [HideInInspector] public int PlayerNum;
    // PlayerNum 0 = Player1 , PN 1 = Player2 , PN 2 = Player3

    public float RecordTimer = 0.0f;

    public Button Replay_Btn;

    [HideInInspector] public float RecordLength;   //���� �÷��� ���� ������ �Ÿ� ����� ����
    List<float> PlayerRecord = new List<float>();  //��� ����� ����Ʈ
    List<float> PlayerScore = new List<float>();   //��� ���� ���Ŀ� ����Ʈ

    //�� �÷��̾� ���� ��߱����� �Ÿ��� �����ϱ� ���� ����Ʈ ����
    List<PlayerData> m_PlayerList = new List<PlayerData>();

    void Start()
    {
        s_State = GameState.Ready;  //static������ ������ �� �ѹ��� �ʱ�ȭ ���ֵ���

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            { 
                SceneManager.LoadScene("GameScene");
            });
        
        this.car = GameObject.Find("car");    //����Ƽ �� ������Ʈ�� �ҷ���
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");  
    }

    // Update is called once per frame
    void Update()
    {
        float length = (this.flag.transform.position.x - this.car.transform.position.x);
        float AbLength = Mathf.Abs(length);
        //���밪
        //if( length < 0)
        //    length = -length;

        this.distance.GetComponent<Text>().text = "Distance : " + AbLength.ToString("F2") + " m";
        RecordLength = AbLength; //��ϰ��� ��� ������ ����
    }

    public void Record()
    {
        this.Player[PlayerNum].GetComponent<Text>().text += RecordLength.ToString("F2") + "m";
        PlayerNum++;
        PlayerRecord.Add(RecordLength);
        PlayerScore.Add(RecordLength);
        
        PlayerData a_Node = new PlayerData();
        a_Node.m_Index = PlayerNum;
        a_Node.m_SvLength = RecordLength;
        m_PlayerList.Add(a_Node);

        if (PlayerScore.Count == 3) //��� ������ �÷��̸� ���� ���
        {
            s_State = GameState.GameEnd;
            Judge(); //���� ����
        }
            
    }

    void Judge()
    {
        PlayerScore.Sort();

        for(int i=0; i<Player.Length; i++)
        {
            if (PlayerScore[0] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 1��";
            }
            else if (PlayerScore[1] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 2��";
            }
            else if (PlayerScore[2] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 3��";
            }
        }
        Replay_Btn.gameObject.SetActive(true); //���÷��� ��ư Ȱ��ȭ

        ////��߱����� �Ÿ�(m_PlayerList[0].m_SvLength)�� �������� �������� ����(ASC)
        //m_PlayerList.Sort(SvLenComp);

        ////�����ؼ� ���
        //PlayerData a_Player = null;
        //for(int i = 0; i < m_PlayerList.Count; i++)
        //{
        //    a_Player = m_PlayerList[i];

        //    if (Player.Length <= a_Player.m_Index)
        //        continue;

        //    a_Player.m_Ranking = i + 1;

        //    Player[a_Player.m_Index].text += " " + a_Player.m_Ranking + "��";
        //}
    }
    
    //int SvLenComp(PlayerData a, PlayerData b)
    //{
    //    return a.m_SvLength.CompareTo(b.m_SvLength);
    //}
}
