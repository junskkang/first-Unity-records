using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MonType
{
    Skeleton = 0,
    Alien,
    Count
}

public class Monster_Ctrl : MonoBehaviourPunCallbacks, IPunObservable
{
    public MonType monType;

    //Hp 
    float curHp = 100;
    float maxHp = 100;
    float netHp = 100;  //��Ʈ��ũ �߰踦 ���� Hp��
    public Image hpBarImg;
    public RectTransform HUDCanvas;

    [HideInInspector] public int m_SpawnIdx = -1;

    //������ ���� ���� ������ ������ Enum ����
    AnimState m_Prestate = AnimState.idle;  //Animation ������ ���� ���� 
    [HideInInspector] public AnimState m_CurState = AnimState.idle;  //�ִϸ��̼� ������ ���� ����

    AnimState MonState = AnimState.idle;    //������ ���� AI ���¸� ������ ����
    //AnimSupporter.cs �ʿ� ���ǵǾ� ����

    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    public Anim anim;  //AnimSupporter.cs �ʿ� ���ǵǾ� ����
    Animation m_RefAnimation;   //Skeleton�� ���Ž� �ִϸ��̼��� �̿��Ͽ� ����
    Animator m_RefAnimator; //Alien�� �ִϸ����͸� �̿��� ����

    //Monster AI
    [HideInInspector] public GameObject m_AggroTarget = null; //������ ���
    int m_AggroTgID = -1;   //�� ���Ͱ� �����ؾ� �� ĳ������ ��Ʈ��ũ �� ���� ��ȣ 
    Vector3 m_MoveDir = Vector3.zero;   //�������� ��� ���� ����
    Vector3 m_CacVLen = Vector3.zero;   //���ΰ��� ���ϴ� ����
    float m_CacDist = 0.0f;             //�Ÿ� ���� ����
    float m_TraceDist = 7.0f;           //���� �ݰ� ����
    float m_AttackDist = 1.8f;          //���� �Ÿ�
    Quaternion m_TargetRot;             //ȸ�� ���� ����
    float m_RotSpeed = 7.0f;            //ȸ�� �ӵ�
    Vector3 m_MoveNextStep = Vector3.zero;  //�̵� �Ի�� ����
    float m_MoveVelocity = 2.0f;        //��� �ʴ� �̵��ӵ�

    public GameObject targetMark = null;
    [HideInInspector] public bool isTarget = false;

    //��Ʈ��ũ ����ȭ�� ���Ͽ�
    Vector3 curPos = Vector3.zero;
    Quaternion curRot = Quaternion.identity;
    int curAnim = 0;
    string m_OldAnim = "";
    PhotonView pv = null;

    void Awake()
    {
        pv = GetComponent<PhotonView>();

        curPos = transform.position;
        curRot = transform.rotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_RefAnimation = GetComponentInChildren<Animation>();
        m_RefAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        
        if (m_CurState == AnimState.die) return;

        if (pv.IsMine)
        {            
            MonStateUpdate();
            MonActionUpdate();
            TargetMark();
        }
        else
        {
            Remote_TrUpdate();
            Remote_TakeDamage();
            Remote_Animation();
        }

        
    }
    public void TargetMark()
    {
        if (isTarget)
        {
            HUDCanvas.gameObject.SetActive(true);
            targetMark.gameObject.SetActive(true);
        }
        else
        {
            HUDCanvas.gameObject.SetActive(false);
            targetMark.gameObject.SetActive(false);
        }
        
    }

    void MonStateUpdate()
    {
        if (m_AggroTarget == null)  //��׷� Ÿ���� �������� ���� ��� ��� ���� ����
        {
            //���ο� ��׷� Ÿ�� ã��
            GameObject[] a_Players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < a_Players.Length; i++)
            {
                //���� ���Ϳ��� �÷��̾ ���ϴ� ����
                m_CacVLen = a_Players[i].transform.position - this.transform.position;
                m_CacVLen.y = 0.0f;
                m_MoveDir = m_CacVLen.normalized;   //���ΰ��� ���ϴ� ���� ����
                m_CacDist = m_CacVLen.magnitude;    //���ΰ������� �����Ÿ�

                if (m_CacDist <= m_AttackDist)  //�÷��̾������ �����Ÿ��� ���ݹ��� ���� ���Դ��� ����
                {
                    MonState = AnimState.attack;
                    m_AggroTarget = a_Players[i];
                    break;
                }
                else if (m_CacDist <= m_TraceDist)  //�÷��̾������ �����Ÿ��� �������� ���� ���Դ��� ����
                {
                    MonState = AnimState.trace;
                    m_AggroTarget = a_Players[i];
                    break;
                }
            }

            if (m_AggroTarget == null)  //for���� �� ���Ƶ� Ÿ���� ���������ٸ�
            {
                MonState = AnimState.idle;
                m_AggroTgID = -1;
            }
        }
        else        //��׷� Ÿ���� ������ ��� ����
        {
            m_CacVLen = m_AggroTarget.transform.position - this.transform.position;
            m_CacVLen.y = 0.0f;
            m_MoveDir = m_CacVLen.normalized;   //���ΰ��� ���ϴ� ����
            m_CacDist = m_CacVLen.magnitude;    //���ΰ������� �����Ÿ�


            if (m_CacDist <= m_AttackDist)  //�÷��̾������ �����Ÿ��� ���ݹ��� ���� ���Դ��� ����
            {
                MonState = AnimState.attack;

            }
            else if (m_CacDist <= m_TraceDist)
            {
                MonState = AnimState.trace;
            }
            else
            {
                MonState = AnimState.idle;
                m_AggroTarget = null;
                m_AggroTgID = -1;
            }
        }

    }

    void MonActionUpdate()
    {
        if (m_AggroTarget == null)
        {
            //Ÿ���� ������ �ִϸ��̼� ���¸� Idle��
            ChangeAnimState(AnimState.idle, 0.12f);
        }
        else  //Ÿ���� �����ϸ�
        {
            if (MonState == AnimState.attack)
            {
                if (0.0001f < m_MoveDir.magnitude)
                {
                    m_TargetRot = Quaternion.LookRotation(m_MoveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, m_RotSpeed * Time.deltaTime);
                }
                ChangeAnimState(AnimState.attack, 0.12f);
            }
            else if (MonState == AnimState.trace) 
            {
                if (0.0001f < m_MoveDir.magnitude)
                {
                    m_TargetRot = Quaternion.LookRotation(m_MoveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, m_RotSpeed * Time.deltaTime);
                }

                //���� ���� �ִϸ��̼� ���̸� ���� �ִϸ��̼��� ���� �� �����ϵ���
                if (IsAttackAnim()) return;

                //�̵�
                m_MoveNextStep = m_MoveDir * Time.deltaTime * m_MoveVelocity;
                m_MoveNextStep.y = 0.0f;
                transform.position += m_MoveNextStep;

                ChangeAnimState(AnimState.trace, 0.12f);
            }
            else if (MonState == AnimState.idle)
            {
                ChangeAnimState(AnimState.idle, 0.12f);
            }
        }
    }

    //�ִϸ��̼� ���� ���� �޼���
    void ChangeAnimState(AnimState newState, float CrossTime = 0.0f)
    {
        if (m_Prestate == newState) return; //���� ���¿� ���ο� ���°� ������ ����

        if (m_RefAnimation != null)
        {
            string a_StrAnim = anim.Idle.name;
            if (newState == AnimState.idle)
                a_StrAnim = anim.Idle.name;
            else if (newState == AnimState.trace)
                a_StrAnim = anim.Move.name;
            else if  (newState == AnimState.attack)
                a_StrAnim = anim.Attack1.name;
            else if (newState == AnimState.die)
                a_StrAnim = anim.Die.name;

            if (0.0f < CrossTime)
                m_RefAnimation.CrossFade(a_StrAnim, CrossTime);
            else
                m_RefAnimation.Play(a_StrAnim);
        }

        if (m_RefAnimator != null)
        {
            m_RefAnimator.ResetTrigger(m_Prestate.ToString());  //������ ����Ǿ� �ִ� Trigger ���� ����

            if (0.0f < CrossTime)
                m_RefAnimator.SetTrigger(newState.ToString());
            else
            {
                string animName = anim.Idle.name;
                if (newState == AnimState.die)
                    animName = anim.Die.name;

                m_RefAnimator.Play(animName, -1, 0);
            }                
        }

        m_Prestate = newState;
        m_CurState = newState;
    }

    //���ݾִϸ��̼� ���� ����
    float m_CacRate = 0.0f;
    float m_NormalTime = 0.0f;

    bool IsAttackAnim() //���� ���� �ִϸ��̼� ������ �ƴ��� üũ�ϴ� �Լ�
    {
        //if (m_RefAnimation == null) return false;

        if (m_RefAnimation != null)
        {
            if (m_RefAnimation.IsPlaying(anim.Attack1.name))
            {
                m_NormalTime = m_RefAnimation[anim.Attack1.name].time /
                                m_RefAnimation[anim.Attack1.name].length;

                //m_RefAnimation[anim.Attack1.name].time : �ִϸ��̼��� ���� ��� �ð���
                //m_RefAnimation[anim.Attack1.name].length : �ִϸ��̼� �� ������ �� �ð���
                //time���� ������ �ɷ������� �ִϸ��̼� �ѵ����� ������ ������������ �� �� ���� �ʱ�ȭ���� �ʰ�
                //��� �����ȴ�.

                //�׷���~ ������ �������� ���ִ� ��
                m_CacRate = m_NormalTime - (int)m_NormalTime;

                if (m_CacRate <= 0.95f)
                    return true;
            }
        }

        if (m_RefAnimator != null)
        {
            //���� �ִϸ��̼� ���� ������ ������
            AnimatorStateInfo stateInfo = m_RefAnimator.GetCurrentAnimatorStateInfo(0);

            //���� ���°� ���� �ִϸ��̼����� üũ
            if (stateInfo.IsName(anim.Attack1.name)) 
            {
                //�ִϸ��̼��� ���൵�� üũ
                m_NormalTime = stateInfo.normalizedTime % 1.0f;

                //�ִϸ��̼��� ���� ���κ��� �ƴ϶�� (95% ����)
                if (m_NormalTime < 0.95f)
                    return true;
            }
        }

        return false;

    }

    public void Event_AttHit()  //�ִϸ��̼� �̺�Ʈ�Լ��� ���� ȣ��
    {
        if (m_AggroTarget == null) return;  //����� ������ ����

        Vector3 a_DistVec = m_AggroTarget.transform.position - transform.position;
        float a_CacLen = a_DistVec.magnitude;
        a_DistVec.y = 0.0f;

        //���ݰ��� �ȿ� �ִ� ���
        if (Vector3.Dot(transform.forward, a_DistVec.normalized) < 0.0f) //���� �ݰ� 90�� ���� �ٱ�
            return;

        //���ݹ��� �ۿ� �ִ� ���
        if ((m_AttackDist + 1.7f) < a_CacLen) return;

        m_AggroTarget.GetComponent<Hero_Ctrl>().TakeDamage(10);
    }

    public void TakeDamage(GameObject a_Attacker, float a_Damage = 10.0f)
    {
        if (curHp <= 0) return;

        if (pv.IsMine)
        {
            curHp -= a_Damage;

            if (curHp < 0)
                curHp = 0;

            if (hpBarImg != null)
                hpBarImg.fillAmount = curHp / maxHp;
        }
        else
        {
            Remote_TakeDamage();
        }

        Vector3 a_CacPos = this.transform.position;
        a_CacPos.y += 1.9f;
        GameMgr.Inst.SpawnDamageText((int)a_Damage, a_CacPos);
        if (pv.IsMine)
        if (curHp <= 0)
        {
            CreateItem();

            //���
            if (monType == MonType.Alien)
            {
                ChangeAnimState(AnimState.die);

                    PhotonNetwork.Destroy(this.gameObject);    //Destroy(this.gameObject, 2.0f);
            }
            else
                    PhotonNetwork.Destroy(this.gameObject);    //Destroy(this.gameObject);
        }
    }

    void Remote_TrUpdate()
    {
        if (5.0f < (transform.position - curPos).magnitude)
        {
            transform.position = curPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10.0f);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, curRot, Time.deltaTime * 10.0f);
    }

    void Remote_TakeDamage()
    {
        if (0.0f < curHp)
        {
            curHp = netHp;
            hpBarImg.fillAmount = curHp / maxHp;

            if (curHp <= 0)
            { 
                curHp = 0.0f;
                //��� ���⸸
            }

        }
        else
        {
            curHp = netHp;
            hpBarImg.fillAmount = curHp / maxHp;
        }
    }

    void Remote_Animation()
    {
        ChangeAnimState(m_CurState, 0.12f);
    }

    //������ ���� �Լ�
    void CreateItem()
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        Vector3 a_HPos = transform.position;
        //a_HPos.y += 1.0f;

        PhotonNetwork.InstantiateRoomObject("DiamondPrefab", a_HPos, Quaternion.identity, 0);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(curHp);

            stream.SendNext((int)m_CurState);
            stream.SendNext(m_SpawnIdx);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            netHp = (float)stream.ReceiveNext();

            m_CurState = (AnimState)stream.ReceiveNext();
            m_SpawnIdx = (int)stream.ReceiveNext();
        }
    }
}
