using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;

public class SpawnPos   // List�� �����ϰ� List�̱� ������ �ε����� ���� ��
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

//������ ��ġ������ �⺻������ ������ Ŭ���̾�Ʈ�� �����Ѵ�.
//������ Ŭ���̾�Ʈ�� ������ ������ �� ��ġ���鵵 ���� ������� �ȴ�.
//�׷��� InstantiateRoomObject�� ����� �ش� ��ġ������ �� �Ҽ��� �ǰԲ� ���־�� 
//Ư�� ��Ȳ������ ������ �� �ִ�.
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
                if (a_SPTList[i].name.Contains("MonSpawnMgr"))  //�ڽĵ鸸 ������ġ�� ������Բ�
                    continue;

                a_Spos = new SpawnPos(a_SPTList[i].position, a_SPTList[i].rotation);
                a_Spos.m_SpawnTime = Random.Range(1.5f, 5.0f); //1.5 ~ 5�� �ڿ� ����
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
        if (PhotonNetwork.CurrentRoom == null) return;  //����ȭ ������ ������ ���� ������Ʈ

        if (!PhotonNetwork.IsMasterClient) return;      //���常 ������Ʈ �����ϰ�

        if (0.0f < g_NetDelay)
            g_NetDelay -= Time.deltaTime;

        if (0.0f < g_NetDelay) return; //������ ����� ������ �ֱ�. ������ ���� ����ȭ�� ����

        if (m_SpawnPos.Count <= 0) return;

        GameObject TempMon = null;
        for (int i = 0; i < m_SpawnPos.Count; i++)
        {
            if (m_SpawnPos[i].m_SpawnTime <= 0.0f)   //�̹� ����Ÿ���� 0�� �Ǿ� ������ �� ��ġ�� ��ŵ
                continue;

            m_SpawnPos[i].m_SpawnTime -= Time.deltaTime;
            if (m_SpawnPos[i].m_SpawnTime <= 0.0f)
            {
                int MonsterKind = Random.Range(0, 2);   //���̷��� or ���ϸ��� ������

                if (MonsterKind == 0)
                {
                    //���ʹ� ������ �����ϴ� ���� �ƴ϶� AI�� ���� �����̱� ������
                    //���� �Ҽ��� �ǰԲ� ����� ��
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
            //PhotonNetwork.IsMasterClient�� ���� �����鼭
            //������ Ŭ���̾�Ʈ�� �ٲ� m_SpawnTime ����
            //�μ��ΰ� �޵��� �̸� �ٸ� ��ǻ�͵鿡�Ե� �߰��س���
            //�߰������� �ʾ��� ��쿡 ������ Ŭ���̾�Ʈ �μ��ΰ� �ÿ� 
            //�̹� ������ �Ϸ�� ��ġ������ �ٽ� �ߺ��Ǿ� �����Ǵ� ������ �߻���

            for (int i = 0; i < m_SpawnPos.Count; i++)
            {
                m_SpawnPos[i].m_SpawnTime = (float)stream.ReceiveNext();
            }

        }
    }

    //������ Ŭ���̾�Ʈ ����� ȣ��Ǵ� �Լ�
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);

        //���ο� ������ Ŭ���̾�Ʈ���� ��� ���� ���¸� ��� �μ��ΰ� ���־�� �Ѵ�.
        g_NetDelay = 1.0f;
    }
}
