using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum PacketType
{
    BestScore,      //�ְ�����
    UserGold,       //�������
    NickUpdate,     //�г��Ӱ���
    InfoUpdate,     //������������, ����������
    FloorUpdate,    //������

    ClearSave       //������ ����� ���� �ʱ�ȭ �ϱ� 
}

public class NetworkMgr : MonoBehaviour
{
    //--- ������ ������ ��Ŷ ó���� ť ���� ����
    bool isNetworkLock = false; //Network ��� ���� ���� ����
    float m_NetWaitTime = 0.0f;
    List<PacketType> m_PacketBuff = new List<PacketType>();
    //--- ������ ������ ��Ŷ ó���� ť ���� ����

    string BestScoreUrl = "";
    string MyGoldUrl = "";
    string InfoUpdateUrl = "";
    string UpdateFloorUrl = "";

    //�̱��� ������ ���� �ν��Ͻ� ���� ����
    public static NetworkMgr Inst = null;

    void Awake()
    {
        Inst = this;   
    }
    //�̱��� ������ ���� �ν��Ͻ� ���� ����

    // Start is called before the first frame update
    void Start()
    {
        BestScoreUrl = "http://junskk.dothome.co.kr/practice/UpdateBScore.php";
        MyGoldUrl = "http://junskk.dothome.co.kr/practice/UpdateMyGold.php";
        InfoUpdateUrl = "http://junskk.dothome.co.kr/practice/InfoUpdate.php";
        UpdateFloorUrl = "http://junskk.dothome.co.kr/practice/UpdateFloor.php";
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

        if (isNetworkLock == false)  //���� ��Ŷ ó�� ���� ���°� �ƴϸ�...
        {
            if(0 < m_PacketBuff.Count)  //��� ��Ŷ�� �����Ѵٸ�...
            {
                Req_Network();
            }
        }//if (isNetworkLock == false)  //���� ��Ŷ ó�� ���� ���°� �ƴϸ�...
    }//void Update()

    void Req_Network()  //RequestNetwork
    {
        if (m_PacketBuff[0] == PacketType.BestScore)
            StartCoroutine(UpdateScoreCo());
        else if (m_PacketBuff[0] == PacketType.UserGold)
            StartCoroutine(UpdateGoldCo());
        else if (m_PacketBuff[0] == PacketType.InfoUpdate)
            StartCoroutine(UpdateInfoCo());
        else if (m_PacketBuff[0] == PacketType.FloorUpdate)
            StartCoroutine(UpdateFloorCo());

        m_PacketBuff.RemoveAt(0);
    }

    IEnumerator UpdateScoreCo()
    {
        if(GlobalValue.g_Unique_ID == "")  //���������� �α��� �Ǿ� ���� �ʴٸ�...
            yield break;        //�ڷ�ƾ �Լ��� ���� ������...

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_score", GlobalValue.g_BestScore);

        isNetworkLock = true;
        m_NetWaitTime = 3.0f;  //3�� ���� ������ ������ ���� ��Ŷ ó�� �����ϵ���...

        UnityWebRequest a_www = UnityWebRequest.Post(BestScoreUrl, form);
        yield return a_www.SendWebRequest();    //������ �ö����� ����ϱ�...

        if(a_www.error == null) //������ ���� �ʾ��� �� ����
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

        yield return a_www.SendWebRequest();    //������ �ö����� ����ϱ�...

        if(a_www.error == null) //������ ���� �ʾ��� �� ����
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

    IEnumerator UpdateInfoCo()
    {
        if (GlobalValue.g_Unique_ID == "") yield break; //�α��� �ȵǾ� ������ ����

        //Json �����
        
        ItemList itemList = new ItemList();
        itemList.SkList = new int[GlobalValue.g_SkillCount.Length];
        for (int i = 0; i < GlobalValue.g_SkillCount.Length; i++)
        {
            itemList.SkList[i] = GlobalValue.g_SkillCount[i];
        }
        //Json ���ڿ��� ��ȯ ���� : {"SkList" : [1,1,0]}
        string strJson = JsonUtility.ToJson(itemList);

        WWWForm form = new WWWForm();
        //Input_user��� �ʵ忡 ����ũID�� UTF8���ڵ����� �Է½�Ű��
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Item_list", strJson, System.Text.Encoding.UTF8);

        isNetworkLock = true;
        m_NetWaitTime = 3.0f;

        UnityWebRequest www = UnityWebRequest.Post(InfoUpdateUrl, form);
        yield return www.SendWebRequest();  //������ �� ������ ����ϱ�

        if (www.error == null)  //������ ���� �ʾҴٸ� == ���� �۵�
        {
            //Debug.Log("Update Success~");
        }
        else
        {
            Debug.Log(www.error);
        }

        www.Dispose();

        isNetworkLock = false;
        m_NetWaitTime = 3.0f;
    }

    IEnumerator UpdateFloorCo()
    {
        if (GlobalValue.g_Unique_ID == "") yield break; //�α��� �ȵǾ� ������ ����

        //Json �����
        FloorInfo a_FInfo = new FloorInfo();
        a_FInfo.CurFloor = GlobalValue.g_CurFloorNum;
        a_FInfo.BestFloor = GlobalValue.g_BestFloor;
        string strJson = JsonUtility.ToJson(a_FInfo);


        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_floor", strJson, System.Text.Encoding.UTF8);

        isNetworkLock = true;

        m_NetWaitTime = 3.0f;

        UnityWebRequest www = UnityWebRequest.Post(UpdateFloorUrl, form);
        yield return www.SendWebRequest();

        if (www.error == null)
        {
            //Debug.Log("UpdateSuccess~");
        }
        else
        {
            Debug.Log(www.error);
        }

        www.Dispose ();

        isNetworkLock = false;
        m_NetWaitTime = 0.0f;
    }

    public void PushPacket(PacketType a_PType)
    {
        bool a_isExist = false;
        for(int i = 0; i < m_PacketBuff.Count; i++)
        {
            if (m_PacketBuff[i] == a_PType) //���� ó�� ���� ���� ��Ŷ�� �����ϸ�
                a_isExist = true;
            //�� �߰����� �ʰ� ���� ������ ��Ŷ���� ������Ʈ �Ѵ�.
        }

        if (a_isExist == false)
            m_PacketBuff.Add(a_PType);
        //��� ���� �� Ÿ���� ��Ŷ�� ������ ���� �߰��Ѵ�.
    }

}
