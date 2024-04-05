using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready = 0,     //플레이어 출발선에서 준비 중
    Move = 1,      //자동차가 움직이고 있는 상태
    GameEnd = 2    //플레이어 3명의 플레이가 모두 끝난 상태
}

public class PlayerData
{
    public int m_Index = 0;          //플레이어 번호
    public float m_SvLength = 0.0f;  //플레이어별 기록 저장
    public int m_Ranking = -1;       //랭킹 변수 -1은 아직 순위가 매겨지지 않았다는 의미
}

public class GameDirector : MonoBehaviour
{
    public static GameState s_State = GameState.Ready;  //어디서나 쉽게 접근할 수 있게 static선언

    GameObject car;
    GameObject flag;
    GameObject distance;

    public Text[] Player;  //플레이어별 기록 Text UI
    [HideInInspector] public int PlayerNum;
    // PlayerNum 0 = Player1 , PN 1 = Player2 , PN 2 = Player3

    public float RecordTimer = 0.0f;

    public Button Replay_Btn;

    [HideInInspector] public float RecordLength;   //지금 플레이 중인 유저의 거리 저장용 변수
    List<float> PlayerRecord = new List<float>();  //기록 저장용 리스트
    List<float> PlayerScore = new List<float>();   //기록 순위 정렬용 리스트

    //각 플레이어 별로 깃발까지의 거리를 저장하기 위한 리스트 변수
    List<PlayerData> m_PlayerList = new List<PlayerData>();

    void Start()
    {
        s_State = GameState.Ready;  //static변수는 시작할 때 한번씩 초기화 해주도록

        if (Replay_Btn != null)
            Replay_Btn.onClick.AddListener(() =>
            { 
                SceneManager.LoadScene("GameScene");
            });
        
        this.car = GameObject.Find("car");    //유니티 상 오브젝트를 불러옴
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");  
    }

    // Update is called once per frame
    void Update()
    {
        float length = (this.flag.transform.position.x - this.car.transform.position.x);
        float AbLength = Mathf.Abs(length);
        //절대값
        //if( length < 0)
        //    length = -length;

        this.distance.GetComponent<Text>().text = "Distance : " + AbLength.ToString("F2") + " m";
        RecordLength = AbLength; //기록값을 멤버 변수로 저장
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

        if (PlayerScore.Count == 3) //모든 유저가 플레이를 끝낸 경우
        {
            s_State = GameState.GameEnd;
            Judge(); //순위 판정
        }
            
    }

    void Judge()
    {
        PlayerScore.Sort();

        for(int i=0; i<Player.Length; i++)
        {
            if (PlayerScore[0] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 1등";
            }
            else if (PlayerScore[1] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 2등";
            }
            else if (PlayerScore[2] == PlayerRecord[i])
            {
                this.Player[i].GetComponent<Text>().text += " 3등";
            }
        }
        Replay_Btn.gameObject.SetActive(true); //리플레이 버튼 활성화

        ////깃발까지의 거리(m_PlayerList[0].m_SvLength)를 기준으로 오름차순 정렬(ASC)
        //m_PlayerList.Sort(SvLenComp);

        ////정리해서 출력
        //PlayerData a_Player = null;
        //for(int i = 0; i < m_PlayerList.Count; i++)
        //{
        //    a_Player = m_PlayerList[i];

        //    if (Player.Length <= a_Player.m_Index)
        //        continue;

        //    a_Player.m_Ranking = i + 1;

        //    Player[a_Player.m_Index].text += " " + a_Player.m_Ranking + "등";
        //}
    }
    
    //int SvLenComp(PlayerData a, PlayerData b)
    //{
    //    return a.m_SvLength.CompareTo(b.m_SvLength);
    //}
}
