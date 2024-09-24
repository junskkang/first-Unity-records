using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//HP ������ ����ȭ�� �ȸ´� ���� ��ġ��
//IsMine �������� �����ϰ� �߰��Ѵ�.
//���� IsMine�� �ƹ�Ÿ�� �߻��ϴ� ĳ���� ������ ���� �ʰ�
//���ڰ� ��� Instantiate�� �ϰ� �ֱ� ������ ��ġ���� ��Ȯ�ϰ� ��ġ���� �ʱ� ������
//�ƹ�Ÿ�� �߻��� ĳ���� ���� ������ ���� ����. �׷��� IsMine�� ������ �����ϴ� ��.
//�����ϰ� �Ѿ� �����浹ó���� �Ϸ���
//�Ѿ��� �¾��� �� IsMine���� �Ѿ��� �¾Ҵٴ� RPC�Լ��� ���
//���⼭ RPC �Լ��� �Ѿ� ������ȣ�� ���� ������ �ߺ� �������� �Ͼ�� �ʵ��� ó���Ѵ�.
//�ƹ�Ÿ�̴� IsMine�̴� �Ѿ˿� ������ ������ �ϴ� IsMine�� �Ѿ��� �¾Ҵٰ�
//��ȣ�� ������ IsMine���� ������ ����� ���� �� Hp�� �߰��Ͽ� ����ȭ�� ������� �Ѵ�.
public class TankDamage : MonoBehaviourPunCallbacks, IPunObservable
{
    //��ũ ���� �� ����ó���� ���� MeshRenderer ������Ʈ �迭
    private MeshRenderer[] _renderers;

    //��ũ ���� ȿ�� �������� ������ ����
    private GameObject expEffect = null;

    //��ũ�� �ʱ� ����ġ
    private int initHp = 200;
    //��ũ�� ���� ����ġ
    [HideInInspector] public int currHp = 0;
    int NetHp = 200;    //�ƹ�Ÿ��ũ�� Hp���� ����ȭ �����ֱ� ���� ����


    //HUD�� ������ ����
    public Canvas hudCanvas;
    public Image hpBarImg;
    Color startColor = Color.white;


    PhotonView pv = null;
    //�÷��̾��� ������ȣ�� ������ ����
    public int PlayerId = -1;

    //Kill Count ����ȭ�� �ʿ��� ����
    //��ũ HUD�� ǥ���� ���ھ� Text UI �׸�
    public Text txtKillCount;

    //�� ��ũ �ı� ���ھ CustomProperties�� ���� �߰��ϱ� ���� ������
    int killCount = 0;
    int lastAttackId = -1;  //��Ÿ�� ���� �ƴ��� ���̵� �޾Ƴ��� ����

    ExitGames.Client.Photon.Hashtable KillProps = new ExitGames.Client.Photon.Hashtable();

    [HideInInspector] public float m_ResetTime = 0.0f;

    private void Awake()
    {
        //��ũ ���� ��� MeshRenderer ������Ʈ�� ������ �� �迭�� �Ҵ�
        _renderers = GetComponentsInChildren<MeshRenderer>();
        
        pv = GetComponent<PhotonView>();

        //���� ����ġ�� �ʱ� ����ġ�� �ʱⰪ ����
        currHp = initHp;
        //��ũ ���߽� ������ų ���� ȿ���� �ε�
        expEffect = Resources.Load("ExplosionMobile") as GameObject;

        startColor = hpBarImg.color;

        PlayerId = pv.Owner.ActorNumber;

        InitCustomProperties(pv);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    int updateCk = 2;
    // Update is called once per frame
    void Update()
    {
        //��ũ�� �濡 ó�� �����ϸ� �Ⱥ��̵��� ó��
        //Ÿ�̹� �� ��� update�� ���� �Ŀ� ����Ǿ�� UI�� ������ �ʴ´�
        if (0 < updateCk) 
        {
            updateCk--;
            if (updateCk <= 0)
            {
                ReadyStateTank();
            }
        }

        if (m_ResetTime > 0.0f)
        {
            m_ResetTime -= Time.deltaTime;
        }

        if (PhotonNetwork.CurrentRoom == null ||
            PhotonNetwork.LocalPlayer == null) return;
        //����ȭ ������ �����϶��� ������Ʈ�� ����� �ش�.
        //�κ�� ���ư� �� �����Ʈ��ũ �뿡�� ���� ������
        //�� ���� �� �̵��� �ϰ� �Ǵµ�
        //�� �̵� ���� ������� �Լ��� ���ư��� �Ǹ� ������ ��
        
        //�ƹ�Ÿ ��ũ�� �� ����ȭ �ڵ�
        if (pv.IsMine == false)
        {
            //���� �÷��̾��϶�
            AvatarUpdate();

            //�ƹ�Ÿ ��ũ�� ���忡�� KillCount �߰� �ޱ�
            ReceiveKillCount();
        }

        if (txtKillCount != null)
            txtKillCount.text = killCount.ToString();   //ų ī��Ʈ UI ����
    }

    public void ReadyStateTank()
    {
        if (GameManager.gameState != GameState.GS_Ready) return;

        StartCoroutine(this.WaitReadyTank());
    }

    //���� ���� ��� ��
    IEnumerator WaitReadyTank()
    {
        //HUD ��Ȱ��ȭ
        hudCanvas.enabled = false;

        //��ũ ����ó��
        SetTankVisible(false);

        while (GameManager.gameState == GameState.GS_Ready)
            yield return null;

        //��ũ Ư���� ��ġ�� ���� �� ����ȭ ����
        float pos = Random.Range(-100.0f, 100.0f);
        Vector3 sitPos = new Vector3(pos, 20.0f, pos);

        string a_TeamKind = ReceiveSelTeam(pv.Owner);
        int a_SitPosIdx = ReceiveSitPosIdx(pv.Owner);
        if (0 <= a_SitPosIdx && a_SitPosIdx < 4)
        {
            if (a_TeamKind == "blue")
            {
                sitPos = GameManager.m_Team1Pos[a_SitPosIdx];
                this.gameObject.transform.eulerAngles = new Vector3(0.0f, 201.0f, 0.0f);
            }
            else if (a_TeamKind == "red")
            {
                sitPos = GameManager.m_Team2Pos[a_SitPosIdx];
                this.gameObject.transform.eulerAngles = new Vector3(0.0f, 19.5f, 0.0f);
            }
        }

        transform.position = sitPos;
        //HUD �ʱ�ȭ
        hpBarImg.fillAmount = 1.0f;
        hpBarImg.color = startColor;
        hudCanvas.enabled = true;

        if (pv != null && pv.IsMine)
            currHp = initHp;

        //������ ���� �ð� �ʱ�ȭ
        m_ResetTime = 5.0f;


        SetTankVisible(true);        
    }

    private void OnTriggerEnter(Collider coll)
    {
        //�浹�� Collider�� �±� ��
        if (currHp > 0 && coll.tag.Contains("Cannon"))
        {
            //�浹�� ĳ���� ���̵� �޾Ƽ� �ѱ��
            int att_Id = -1;
            Cannon refCannon = coll.gameObject.GetComponent<Cannon>();
            if ((refCannon) != null)
                att_Id = refCannon.AttackerId;

            if ((string)pv.Owner.CustomProperties["MyTeam"] == refCannon.teamColor)
                return;

            TakeDamage(att_Id);

            //Debug.Log((string)pv.Owner.CustomProperties["MyTeam"] + " : " + refCannon.teamColor);
            

            //Debug.Log("�浹�� �ǳ�?");

            //currHp -= 20;

            ////���� ����ġ HUD�� ǥ��
            //hpBarImg.fillAmount = (float)currHp / (float)initHp;
            ////���� ��ġ�� ���� ü�¹� ���� ����
            //if (hpBarImg.fillAmount <= 0.5f)
            //    hpBarImg.color = Color.yellow;

            //if (hpBarImg.fillAmount <= 0.3f)
            //    hpBarImg.color = Color.red;

            //if (currHp <= 0)
            //{
            //    StartCoroutine(this.ExplosionTank());
            //}
        }
    }

    public void TakeDamage(int Attacker = -1)
    {
        //�ڱⰡ �� �Ѿ��� �ڱⰡ ������ �ȵǵ��� ó��
        if (Attacker == PlayerId) return;

        if (currHp <= 0.0f) return;

        //�ǰݿ���

        if (!pv.IsMine) return;     //�� �Լ��� IsMine�� ����ǵ��� ����ó��

        if (0.0f < m_ResetTime) return; //������ �� ����Ÿ��



        //pv.IsMine�϶�
        lastAttackId = Attacker;
        currHp -= 20;
        if(currHp <0)
            currHp = 0;
        
        //���� ����ġ HUD�� ǥ��
        hpBarImg.fillAmount = (float)currHp / (float)initHp;
        //���� ��ġ�� ���� ü�¹� ���� ����
        if (hpBarImg.fillAmount <= 0.5f)
            hpBarImg.color = Color.yellow;

        if (hpBarImg.fillAmount <= 0.3f)
            hpBarImg.color = Color.red;

        if (currHp <= 0)    //�״� ó�� (�ƹ�Ÿ ��ũ���� �߰� �޾Ƽ� ó��)
            StartCoroutine(this.ExplosionTank());


    }
    //���� ȿ�� ���� �� ������ �ڷ�ƾ �Լ�
    IEnumerator ExplosionTank()
    {
        //���� ȿ�� ����
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);

        Destroy(effect, 3.0f);

        //HUD�� ��Ȱ��ȭ
        hudCanvas.enabled = false;

        //��ũ ���� ó��
        SetTankVisible(false); yield return null;

        //if (pv != null && pv.IsMine)    //IsMine�϶��� ���⸦ ���� �ǻ츮��
        //{
        //    //3�ʵ��� ��ٷȴٰ� Ȱ��ȭ�ϴ� ������ ����
        //    yield return new WaitForSeconds(5.0f);

        //    //HUD �ʱ�ȭ
        //    hpBarImg.fillAmount = 1.0f;
        //    hpBarImg.color = startColor;
        //    hudCanvas.enabled = true;

        //    //������ �� ���� �ʱⰪ ����
        //    currHp = initHp;

        //    //������ ���� �ð� �ʱ�ȭ
        //    m_ResetTime = 5.0f;

        //    //��ũ�� �ٽ� ���̰� ó��
        //    SetTankVisible(true);
        //}
        //else
        //{   //�ƹ�Ÿ ��ũ���� �߰�޾Ƽ� �ǻ츮�� ���� �ƹ��͵� ���� �ʰ� ������
        //    yield return null;
        //}

    }

    //MeshRenderer�� Ȱ��/��Ȱ��ȭ�ϴ� �Լ�
    void SetTankVisible(bool isVisible)
    {
        foreach (MeshRenderer renderer in _renderers)
        {
            renderer.enabled = isVisible;
        }

        Rigidbody[] rigids = GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rig in rigids)
        {
            rig.isKinematic = !isVisible;
        }

        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>(true);
        foreach (BoxCollider boxCollider in boxColliders)
        {
            boxCollider.enabled = isVisible;
        }

        //if (isVisible && pv.IsMine)
        //{
        //    float pos = Random.Range(-100.0f, 100.0f);
        //    transform.position = new Vector3(pos, 20.0f, pos);
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lastAttackId);  //�̹��� -20���ΰ� ���� �������� �������� �ǵ�
            stream.SendNext(currHp);
        }
        else
        {
            lastAttackId = (int)stream.ReceiveNext();   //�ƹ�Ÿ�鵵 ���������� �ǰ� �𿴴��� ����
            NetHp = (int)stream.ReceiveNext();
            //�ƹ�Ÿ ���忡�� ��� ������ �˱� ���� NetHp��� ������ ������ �����
            //IsMine���� �۽����� Hp���� �޾ƿ�
        }
    }

    void AvatarUpdate()     //�ƹ�Ÿ�� Hp Update ó�� �Լ�
    {
        if (0 < currHp)
        {
            currHp = NetHp;
            //���� ����ġ ����� 
            hpBarImg.fillAmount = (float)currHp / (float)initHp;

            //���� ��ġ�� ���� ü�¹� ���� ����
            if (hpBarImg.fillAmount <= 0.5f)
                hpBarImg.color = Color.yellow;

            if (hpBarImg.fillAmount <= 0.3f)
                hpBarImg.color = Color.red;

            if (currHp <= 0) //�״�ó�� (�ƹ�Ÿ ��ũ���� �߰� �޾Ƽ� ó��)
            {
                currHp = 0;

                if (0 <= lastAttackId)  //�������� id�� ��ȿ�� ��
                {
                    //������ ��Ÿ�� ģ ��ũ�� ���������� �ľ��ؼ� 
                    //KillCount�� �÷������.
                    //�ڽ��� �ı���Ų �� ��ũ�� ���ھ ������Ű�� �Լ��� ȣ��
                    //'�״� ��ũ ����'���� ���� ���� ����� �ٸ� �÷��̾��� '�ƹ�Ÿ'��
                    //�׷��� �� ���ؿ��� �����ϴ� �ƹ�Ÿ�� �߿��� lastAttackId�� ������
                    //���̵� ���� �ִ� IsMine�� ã�Ƽ� �װ����� ų ī��Ʈ�� �ø���
                    //�Լ��� ȣ�������� ��.                   
                    SaveKillCount(lastAttackId);
                }
                StartCoroutine(ExplosionTank());
            }
        }
        else
        {   //�׾� ���� �� ��� NetHp�� 0���� ��� ������ �ǰ�
            //�ǻ���� �ϴ� ��Ȳ ó��
            currHp = NetHp;
            if ((int)(initHp * 0.95) < currHp)  
            {
                //�̹��� ���� Hp�� �ִ� �������� ������ �ǻ�����ϴ� ������ �Ǵ�

                //Filled �̹��� �ʱⰪ���� ȯ��
                hpBarImg.fillAmount = 1.0f;
                //�̹��� ���� ����
                hpBarImg.color = startColor;
                //HUD Ȱ��ȭ 
                hudCanvas.enabled = true;

                //�������� ���������� ����
                currHp = initHp;

                //������ ���� �ð� �ʱ�ȭ
                m_ResetTime = 5.0f;

                //��ũ�� �ٽ� ���̰� ó��
                SetTankVisible(true);

            }
        }
    }

    //�ڽ��� �ı���Ų �� ��ũ�� ã�Ƽ� ���ھ ���������ִ� �Լ�
    void SaveKillCount(int AttackerId)
    {
        //��ũ �±׷� ������ ��� ������Ʈ�� ������ �迭�� ����
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("TANK");
        foreach (GameObject tank in tanks)
        {
            var tankDamage = tank.GetComponent<TankDamage>();
            if (tankDamage != null && tankDamage.PlayerId == AttackerId)
            {
                //��ũ�� PlayerId�� ��ź�� AttackerID�� �������� �Ǵ�
                if (tankDamage.IncKillCount())
                {
                    return;
                }
            }
        }
    }

    public bool IncKillCount()  
    {
        //���� ��ũ IsMine ���忡�� �� �Լ��� ȣ��Ǿ�� �Ѵ�.
        if (pv != null && pv.IsMine)
        {
            //IsMine �ѱ������� KillCount�� ������Ű�� ������
            //IsMine�� �ƹ�Ÿ�� ���о��� ��� ���� KillCount�� �������� �߰��ϴٺ���
            //KillCount�� ��߳� �� �ֱ� ����
            killCount++;

            //IsMine�϶��� �߰���
            SendKillCount(killCount);

            return true;
        }
        
        return false;
    }

    void InitCustomProperties(PhotonView pv)
    {
        //���۸� �̸� ����� ���� ���� �Լ�
        if (pv != null && pv.IsMine)
        {
            //pv.IsMine == true ���� �����ϰ� �ִ� ��ũ�̰� ���� ������ �ʱ�ȭ
            KillProps.Clear();
            KillProps.Add("KillCount", 0);  //Ű���� ����� �̷���� ����
            pv.Owner.SetCustomProperties(KillProps);
        }
    }

    void SendKillCount(int killCount = 0)
    {
        if (pv == null) return;

        if (!pv.IsMine) return; //IsMine������ �߰踦 ����

        if (KillProps == null)
        {
            KillProps = new ExitGames.Client.Photon.Hashtable();
            KillProps.Clear();
        }

        if (KillProps.ContainsKey("KillCount"))
            KillProps["KillCount"] = killCount;
        else
            KillProps.Add("KillCount", killCount);

        pv.Owner.SetCustomProperties(KillProps);    //�߰��ϴ� ��   
    }

    void ReceiveKillCount()
    {
        if(pv == null) return;

        if(pv.IsMine) return;   //�ƹ�Ÿ�� ��ũ�鸸 �ް� ��

        if (pv.Owner == null) return;

        if (pv.Owner.CustomProperties.ContainsKey("KillCount"))
        {
            killCount = (int)pv.Owner.CustomProperties["KillCount"];
        }
    }

    private int ReceiveSitPosIdx(Player a_Player)
    {
        int a_SitPosidx = -1;

        if (a_Player == null)
            return a_SitPosidx;

        if (a_Player.CustomProperties.ContainsKey("SitPosInx"))
        {
            a_SitPosidx = (int)a_Player.CustomProperties["SitPosInx"];
        }

        return a_SitPosidx;
    }

    private string ReceiveSelTeam(Player a_Player)
    {
        string a_TeamKind = "blue";

        if (a_Player == null)
            return a_TeamKind;

        if (a_Player.CustomProperties.ContainsKey("MyTeam"))
        {
            a_TeamKind = (string)a_Player.CustomProperties["MyTeam"];
        }

        return a_TeamKind;
    }

}
