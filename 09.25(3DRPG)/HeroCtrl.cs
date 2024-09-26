using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCtrl : MonoBehaviour
{
    //Ű���� �̵� ���� ����
    float h = 0, v = 0;
    Vector3 moveVec = Vector3.zero;
    float m_CacRotY = 0.0f;
    float m_RotKbSpeed = 100.0f;
    float m_RunVelocity = 5.0f;
    float m_WalkVelocity = 2.0f;
    bool isRun = false;

    //Picking ���� ����
    Ray m_MouseRay;
    RaycastHit hitInfo;
    LayerMask LayerMask = -1;

    //mouse �̵� ���� ����
    Vector3 m_TargetPos = Vector3.zero;     //�̵��� ��ǥ ��ǥ
    bool m_IsPickMoveOnOff = false;         //���콺 ��ŷ �̵� OnOff
    Vector3 m_MoveDir = Vector3.zero;       //�̵��� ���� ��ǥ
    float m_MoveDurTime = 0.0f;             //�̵��������� �ҿ�Ǵ� �ð� = �Ÿ�/�ӵ�
    double m_AddTimeCount = 0.0f;           //�����ð� ī��Ʈ
    Vector3 m_CacLenVec = Vector3.zero;    //�̵� ���� ����
    Quaternion m_TargetRot = Quaternion.identity;   //ȸ�� ���� ����
    float m_RotSpeed = 10.0f;

    //���̽�ƽ �̵� ���� ����
    float joyMoveLen = 0.0f;
    Vector3 joyMoveDir = Vector3.zero;

    //�ִϸ��̼� ���� ����
    Animator anim;
    AnimState preState = AnimState.idle;
    AnimState curState = AnimState.idle;

    //���ݰ��� ����
    [HideInInspector] public bool isAttack = false;
    float attackSpeed = 2.0f;

    private void Awake()
    {
        CameraCtrl a_Cam = Camera.main.GetComponent<CameraCtrl>();
        if (a_Cam != null)
            a_Cam.InitCamera(this.gameObject);

        //if(pv.IsMine) ���� �ٲ� ����
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
            gm.InitManager(this.gameObject);

        anim = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        LayerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        KeyControl();
        MousePickCheck();

        KeyboardMove();
        MousePickUpdate();
        JoystickUpdate();

        UpdateAnimState();

        Attack();
    }

    void KeyControl()
    {
        h = Input.GetAxisRaw("Horizontal"); //�¿� ȸ��
        v = Input.GetAxisRaw("Vertical");   //���� �̵�


        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRun = !isRun;

        if (Input.GetKeyDown(KeyCode.Space) && !isAttack)
        {
            if (h != 0.0f || v != 0.0f) return;

            isAttack = true;
        }
;
    }

    void KeyboardMove()
    {
        //if (v < 0.0f)   //���� �ִϸ��̼��� ��� ���� ����
        //    v = 0.0f;

        if (h != 0.0f || v != 0.0f)
        {
            ClearMousePickMove();

            //�¿�� ȸ���� �̵��� �������θ�
            //m_CacRotY = transform.eulerAngles.y;
            //m_CacRotY += h * m_RotKbSpeed * Time.deltaTime;
            //transform.eulerAngles = new Vector3(0.0f, m_CacRotY, 0.0f);

            moveVec = new Vector3(h, 0.0f, v);
            moveVec.y = 0.0f;

            Quaternion quaternion = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, m_RotSpeed * Time.deltaTime);
            if (isRun)
                transform.position += moveVec.normalized * m_RunVelocity * Time.deltaTime;
            else if (!isRun)
                transform.position += moveVec.normalized * m_WalkVelocity * Time.deltaTime;
            //anim.SetTrigger("isRun");
        }
        else
        {
            //anim.SetTrigger("isIdle");
        }
    }

    void MousePickCheck()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.IsPointerOverUIObject())
        {
            m_MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(m_MouseRay, out hitInfo, Mathf.Infinity, LayerMask.value))
            {
                MousePicking(hitInfo.point);
            }
        }
    }

    void MousePicking(Vector3 a_PickVec, GameObject a_PickMon = null)    //���콺 Ŭ�� ó�� �Լ�
    {
        a_PickVec.y = transform.position.y;         //��ǥ��ġ
        Vector3 a_StartPos = transform.position;    //������ġ

        m_CacLenVec = a_PickVec - a_StartPos;
        m_CacLenVec.y = 0.0f;   //Y�� �̵� ����

        if (m_CacLenVec.magnitude < 0.5f) return;   //�ٰŸ� �̵��� ����

        m_TargetPos = a_PickVec;        //���� �̵� ��ġ
        m_IsPickMoveOnOff = true;       //��ŷ �̵� On/Off

        m_MoveDir = m_CacLenVec.normalized;
        m_MoveDurTime = m_CacLenVec.magnitude / m_RunVelocity; //�����ϴµ����� �ɸ��� �ð� = �Ÿ�/�ӵ�
        m_AddTimeCount = 0.0f;
    }

    void MousePickUpdate()
    {
        if (m_IsPickMoveOnOff)
        {
            m_CacLenVec = m_TargetPos - transform.position;
            m_CacLenVec.y = 0.0f;

            //������ ����Ͽ����� �ѹ� �� ������ִ� ������
            //������ ������ �ش� ������ �̵��ϴٰ� ĳ������ ��ġ�� �ٲ������� (���Ϳ� ���� ��)
            //�������� �зȴµ� ������ �״���̱� ������ ���������� �����ϴ� ������ �޶��� �� �ִ�.
            //��� �ѹ� �� ������־� ������ ��Ȯ�� ���شٰ� �������ָ� ����.
            m_MoveDir = m_CacLenVec.normalized;

            //ĳ���͸� �̵��������� ȸ����Ű�� �ڵ�
            if (0.0001f < m_CacLenVec.magnitude)
            {
                m_TargetRot = Quaternion.LookRotation(m_MoveDir);  //�ش� ���Ϳ� ��� ������ ���Ϸ��ޱ۷�
                //Debug.Log(m_TargetRot);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, Time.deltaTime * m_RotSpeed);
            }

            m_AddTimeCount += Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount) //��ǥ������ ������ ������ ����
            {
                ClearMousePickMove();
                //anim.SetTrigger("isIdle");
            }
            else
            {
                if (isRun)
                    transform.position += m_MoveDir * Time.deltaTime * m_RunVelocity;
                else if (!isRun)
                    transform.position += m_MoveDir * Time.deltaTime * m_WalkVelocity;
                //anim.SetTrigger("isRun");
            }

        }
    }

    void ClearMousePickMove()
    {
        m_IsPickMoveOnOff = false;


        //���콺 Ŭ����ũ ���
    }

    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        joyMoveLen = a_JoyMvLen;
        if (0.0f < a_JoyMvLen)
        {
            //joyMoveDir = new Vector3(a_JoyMvDir.x, 0.0f, a_JoyMvDir.y);

            //ī�޶� �ٶ󺸴� �������� �׻� �̵� ��Ű�� 
            Vector3 camForward = Camera.main.transform.forward;
            camForward.y = 0.0f;
            camForward.Normalize();
            joyMoveDir = camForward * a_JoyMvDir.y;

            Vector3 camRight = Camera.main.transform.right;
            joyMoveDir += camRight * a_JoyMvDir.x;
            joyMoveDir.y = 0.0f;
            joyMoveDir.Normalize();
        }
        else
        {
            //anim.SetTrigger("isIdle");
        }
    }

    public void JoystickUpdate()
    {
        if (h != 0.0f || v != 0.0f) return;

        if (0.0f < joyMoveLen)
        {
            ClearMousePickMove();

            if (isRun)
                transform.Translate(joyMoveDir * joyMoveLen * m_RunVelocity * Time.deltaTime, Space.World);
            else if (!isRun)
                transform.Translate(joyMoveDir * joyMoveLen * m_WalkVelocity * Time.deltaTime, Space.World);
            m_TargetRot = Quaternion.LookRotation(joyMoveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, Time.deltaTime * m_RotSpeed);
            //anim.SetTrigger("isRun");
        }
    }

    public void Attack()
    {
        if (isAttack)
        {
            ClearMousePickMove();
            moveVec = Vector3.zero;

            attackSpeed -= Time.deltaTime;
            if (attackSpeed < 0)
            {
                attackSpeed = 2.0f;
                isAttack = false;
            }
                
        }
    }

    public void ChangeAnimState(AnimState newState, float crossTime = 0.12f, string animName = "")
    {
        if (anim == null) return;

        if (preState == newState) return;

        anim.ResetTrigger(preState.ToString()); //�ش� Ʈ���Ÿ� ��ҽ�Ű�� �ɼ�

        if (0.0 < crossTime)    //�����ð��� �����ϸ�
        {
            anim.SetTrigger(newState.ToString());
        }
        else
        {
            anim.Play(animName, -1, 0);
            //��� �μ� -1 : Layer Index
            //������ �μ� 0 : �ִϸ��̼��� ó������ �ٽ� �����Ű�ڴٴ� �ǹ�
        }

        preState = newState;
        curState = newState;
    }

    void UpdateAnimState()  //�ִϸ��̼� ���� ������Ʈ �޼���
    {
        if ((h != 0.0f || v != 0.0f) || m_IsPickMoveOnOff || 0.0f < joyMoveLen)  //Ű���� �̵����̰ų� ���콺 �̵� ���̸�...
        {
            if (isRun)
                ChangeAnimState(AnimState.run);
            else if (!isRun)
                ChangeAnimState(AnimState.walk);
        }
        else if (isAttack)
        {
            ChangeAnimState(AnimState.attack);
            
        }
        else
        {
            ChangeAnimState(AnimState.idle);
        }
        

    }



}
