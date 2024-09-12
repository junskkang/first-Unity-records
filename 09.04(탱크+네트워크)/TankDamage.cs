using Photon.Pun;
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
    int PlayerId = -1;

    //�� ��ũ �ı� ���ھ CustomProperties�� ���� �߰��ϱ� ���� ������
    //int killCount = 0;
    int lastAttackId = -1;  //��Ÿ�� ���� �ƴ��� ���̵� �޾Ƴ��� ����

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
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        }
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

            TakeDamage(att_Id);

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
        SetTankVisible(false);
        
        if (pv != null && pv.IsMine)    //IsMine�϶��� ���⸦ ���� �ǻ츮��
        {
            //3�ʵ��� ��ٷȴٰ� Ȱ��ȭ�ϴ� ������ ����
            yield return new WaitForSeconds(5.0f);

            //HUD �ʱ�ȭ
            hpBarImg.fillAmount = 1.0f;
            hpBarImg.color = startColor;
            hudCanvas.enabled = true;

            //������ �� ���� �ʱⰪ ����
            currHp = initHp;
            //��ũ�� �ٽ� ���̰� ó��
            SetTankVisible(true);
        }
        else
        {   //�ƹ�Ÿ ��ũ���� �߰�޾Ƽ� �ǻ츮�� ���� �ƹ��͵� ���� �ʰ� ������
            yield return null;
        }

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
            rig.isKinematic = isVisible;
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
                    //
                }
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
                //��ũ�� �ٽ� ���̰� ó��
                SetTankVisible(true);

            }
        }
    }

}
