using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum PacketType
{
    BestScore,      //최고점수
    UserGold,       //유저골드
    NickUpdate,     //닉네임갱신
    InfoUpdate,     //각종정보갱신, 아이템정보
    FloorUpdate,    //층정보

    ClearSave       //서버에 저장된 내용 초기화 하기 
}

public class NetworkMgr : MonoBehaviour
{
    //--- 서버에 전송할 패킷 처리용 큐 관련 변수
    bool isNetworkLock = false; //Network 대기 상태 여부 변수
    float m_NetWaitTime = 0.0f;
    List<PacketType> m_PacketBuff = new List<PacketType>();
    //--- 서버에 전송할 패킷 처리용 큐 관련 변수

    string BestScoreUrl = "";
    string MyGoldUrl = "";
    string InfoUpdateUrl = "";
    string UpdateFloorUrl = "";

    //싱글턴 패텬을 위한 인스턴스 변수 선언
    public static NetworkMgr Inst = null;

    void Awake()
    {
        Inst = this;   
    }
    //싱글턴 패텬을 위한 인스턴스 변수 선언

    // Start is called before the first frame update
    void Start()
    {
        BestScoreUrl = "http://http://junskk.dothome.co.kr/practice/UpdateBScore.php";
        MyGoldUrl = "http://http://junskk.dothome.co.kr/practice/UpdateMyGold.php";
        InfoUpdateUrl = "http://http://junskk.dothome.co.kr/practice/InfoUpdate.php";
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_NetWaitTime)
        {
            m_NetWaitTime -= Time.unscaledDeltaTime;
            if (m_NetWaitTime <= 0.0f)
            {
                isNetworkLock = false;
                //Debug.Log("isNetworkLock = false");
            }
        }//if (0.0f < m_NetWaitTime)

        if (isNetworkLock == false)  //지금 패킷 처리 중인 상태가 아니면...
        {
            if(0 < m_PacketBuff.Count)  //대기 패킷이 존재한다면...
            {
                Req_Network();
            }
        }//if (isNetworkLock == false)  //지금 패킷 처리 중인 상태가 아니면...
    }//void Update()

    void Req_Network()  //RequestNetwork
    {
        if (m_PacketBuff[0] == PacketType.BestScore)
            StartCoroutine(UpdateScoreCo());
        else if (m_PacketBuff[0] == PacketType.UserGold)
            StartCoroutine(UpdateGoldCo());

        m_PacketBuff.RemoveAt(0);
    }

    IEnumerator UpdateScoreCo()
    {
        if(GlobalValue.g_Unique_ID == "")  //정상적으로 로그인 되어 있지 않다면...
            yield break;        //코루틴 함수를 빠져 나가기...

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_score", GlobalValue.g_BestScore);

        isNetworkLock = true;
        m_NetWaitTime = 3.0f;  //3초 동안 응답이 없으면 다음 패킷 처리 가능하도록...

        UnityWebRequest a_www = UnityWebRequest.Post(BestScoreUrl, form);
        yield return a_www.SendWebRequest();    //응답이 올때까지 대기하기...

        if(a_www.error == null) //에러가 나지 않았을 때 동작
        {
            //Debug.Log("UpdateSuccess~~");
        }
        else
        {
            Debug.Log(a_www.error);
        }

        a_www.Dispose();

        isNetworkLock = false;
        m_NetWaitTime = 0.0f;
    }

    IEnumerator UpdateGoldCo()
    {
        if (GlobalValue.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID,
                                    System.Text.Encoding.UTF8);
        form.AddField("Input_gold", GlobalValue.g_UserGold);

        isNetworkLock = true;
        m_NetWaitTime = 3.0f;

        UnityWebRequest a_www = UnityWebRequest.Post(MyGoldUrl, form);

        yield return a_www.SendWebRequest();    //응답이 올때까지 대기하기...

        if(a_www.error == null) //에러가 나지 않았을 때 동작
        {
            //Debug.Log("UpdateGoldSuccess");
        }
        else
        {
            Debug.Log(a_www.error);
        }

        a_www.Dispose();    

        isNetworkLock = false;
        m_NetWaitTime = 0.0f;

    }

    public void PushPacket(PacketType a_PType)
    {
        bool a_isExist = false;
        for(int i = 0; i < m_PacketBuff.Count; i++)
        {
            if (m_PacketBuff[i] == a_PType) //아직 처리 되지 않은 패킷이 존재하면
                a_isExist = true;
            //또 추가하지 않고 기존 버퍼의 패킷으로 업데이트 한다.
        }

        if (a_isExist == false)
            m_PacketBuff.Add(a_PType);
        //대기 중인 이 타입의 패킷이 없으면 새로 추가한다.
    }

}
