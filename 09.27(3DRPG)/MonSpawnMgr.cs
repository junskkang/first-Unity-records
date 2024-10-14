using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;

public class SpawnPos   // List로 관리하고 List이기 때문에 인덱스를 갖게 됨
{
    public Vector3 m_Pos = Vector3.zero;
    public Quaternion m_Rot = Quaternion.identity;
    public float m_SpawnTime = 0.0f;

    public SpawnPos(Vector3 a_Pos, Quaternion a_Rot)
    {
        m_Pos = a_Pos;
        m_Rot = a_Rot;
    }
}

//지형에 설치형들은 기본적으로 마스터 클라이언트가 제어한다.
//마스터 클라이언트가 접속을 해제할 시 설치형들도 같이 사라지게 된다.
//그래서 InstantiateRoomObject로 만들어 해당 설치형들이 룸 소속이 되게끔 해주어야 
//특정 상황에서도 유지될 수 있다.
public class MonSpawnMgr : MonoBehaviourPunCallbacks, IPunObservable
{
    PhotonView pv;
    [HideInInspector] public List<SpawnPos> m_SpawnPos = new List<SpawnPos>();

    float g_NetDelay = 0.0f;

    public static MonSpawnMgr inst;
    private void Awake()
    {
        inst = this;

        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform[] a_SPTList = gameObject.GetComponentsInChildren<Transform>();
        if (a_SPTList != null)
        {
            SpawnPos a_Spos = null;
            for (int i = 0; i < a_SPTList.Length; i++)
            {
                if (a_SPTList[i].name.Contains("MonSpawnMgr"))  //자식들만 스폰위치로 골라지게끔
                    continue;

                a_Spos = new SpawnPos(a_SPTList[i].position, a_SPTList[i].rotation);
                a_Spos.m_SpawnTime = Random.Range(1.5f, 5.0f); //1.5 ~ 5초 뒤에 스폰
                m_SpawnPos.Add(a_Spos);
            }

            Debug.Log(m_SpawnPos.Count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MonSpawnUpdate();
    }

    void MonSpawnUpdate()
    {
        if (PhotonNetwork.CurrentRoom == null) return;  //동기화 가능한 상태일 때만 업데이트

        if (!PhotonNetwork.IsMasterClient) return;      //방장만 업데이트 가능하게

        if (0.0f < g_NetDelay)
            g_NetDelay -= Time.deltaTime;

        if (0.0f < g_NetDelay) return; //마스터 변경시 딜레이 주기. 마스터 정보 동기화를 위해

        if (m_SpawnPos.Count <= 0) return;

        GameObject TempMon = null;
        for (int i = 0; i < m_SpawnPos.Count; i++)
        {
            if (m_SpawnPos[i].m_SpawnTime <= 0.0f)   //이미 스폰타임이 0이 되어 스폰이 된 위치는 스킵
                continue;

            m_SpawnPos[i].m_SpawnTime -= Time.deltaTime;
            if (m_SpawnPos[i].m_SpawnTime <= 0.0f)
            {
                int MonsterKind = Random.Range(0, 2);   //스켈레톤 or 에일리언 두종류

                if (MonsterKind == 0)
                {
                    //몬스터는 누군가 조종하는 것이 아니라 AI를 통해 움직이기 때문에
                    //방의 소속이 되게끔 해줘야 함
                    TempMon = PhotonNetwork.InstantiateRoomObject("Skeleton_Root", 
                        m_SpawnPos[i].m_Pos, m_SpawnPos[i].m_Rot, 0);
                }
                else if (MonsterKind == 1) 
                {
                    TempMon = PhotonNetwork.InstantiateRoomObject("Alien_Root", 
                        m_SpawnPos[i].m_Pos, m_SpawnPos[i].m_Rot, 0);
                }

                if (TempMon != null)
                {
                    TempMon.GetComponent<Monster_Ctrl>().m_SpawnIdx = i;
                }

            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            for (int i = 0; i < m_SpawnPos.Count; i++)
            {
                stream.SendNext(m_SpawnPos[i].m_SpawnTime);
            }
        }
        else
        {
            //PhotonNetwork.IsMasterClient가 방을 나가면서
            //마스터 클라이언트가 바뀌어도 m_SpawnTime 역시
            //인수인계 받도록 미리 다른 컴퓨터들에게도 중계해놓음
            //중계해주지 않았을 경우에 마스터 클라이언트 인수인계 시에 
            //이미 스폰이 완료된 위치에서도 다시 중복되어 스폰되는 현상이 발생함

            for (int i = 0; i < m_SpawnPos.Count; i++)
            {
                m_SpawnPos[i].m_SpawnTime = (float)stream.ReceiveNext();
            }

        }
    }

    //마스터 클라이언트 변경시 호출되는 함수
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        //새로운 마스터 클라이언트에게 모든 변수 상태를 즉시 인수인계 해주어야 한다.
        g_NetDelay = 1.0f;
    }
}
