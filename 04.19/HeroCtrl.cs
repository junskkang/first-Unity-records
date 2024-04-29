using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    //Ű���� �̵� ���� ����
    float h, v;                   //Ű���� �Է°��� �ޱ� ���� ����
    float m_MoveSpeed = 10.0f;    //�ʴ� �̵��ӵ�

    Vector3 m_DirVec;             //�̵��Ϸ��� ���� ���� ����


    //��ǥ ���� ������
    Vector3 m_CurPos;
    Vector3 m_CacEndVec;

    //�Ѿ� �߻� ���� ����
    float m_AttSpeed = 0.1f;    //���ݼӵ�
    float m_CacAttTick = 0.0f;  //����� �߻� �ֱ� �����
    float m_ShootRange = 30.0f; //��Ÿ�

    //ĳ���� ȭ�� ����� ������ ����
    float maxX = 0.98f;
    float minX = 0.02f;
    float maxY = 0.93f;
    float minY = 0.08f;

    //JoyStick �̵� ó�� ����
    float m_JoyMvLen = 0.0f;                //���̽�ƽ ����� ��
    Vector3 m_JoyMvDir = Vector3.zero;      //���̽�ƽ ����� ����

    //���콺 Ŭ�� �̵� ���� ���� (Mouse Picking Move)
    [HideInInspector] public bool m_bMoveOnOff = false; //���� ���콺 ��ŷ���� �̵� ������
    Vector3 m_TargetPos;    //���콺 ��ŷ ��ǥ��
    float m_CacStep;    //�ѽ��� ���� ����
    
    Vector3 m_PickVec = Vector3.zero;
    public ClickMark m_ClickMark;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        MousePickCtrl();

        KeyBoardUpdate();       //Ű�����̵� 1����

        JoyStickMvUpdate();     //���̽�ƽ�̵� 2����

        MousePickUpdate();      //���콺�̵� 3����

        //�Ѿ� �߻� �ڵ�
        if ( 0.0f < m_CacAttTick)       //�Ѿ� ����
            m_CacAttTick -= Time.deltaTime;

        if (Input.GetMouseButton(1) == true)    //���콺 ��Ŭ�� �Ѿ� �߻�
        {
            if (m_CacAttTick <= 0.0f)
            {
                //���콺�� ��ǥ�� ������ǥ��� �޾Ƽ� �� ���͸� �Ű������� ����
                ShootFire(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                m_CacAttTick = m_AttSpeed;
            }
        }
    }

    #region ---- Ű���� �̵�
    void KeyBoardUpdate()   //Ű���� �̵�ó��
    {
        h = Input.GetAxisRaw("Horizontal"); // -1 ~ 1
        v = Input.GetAxisRaw("Vertical");   // -1 ~ 1

        if (h != 0.0f || v != 0.0f)  //�̵� Ű���带 �����ϰ� ������
        {
            m_DirVec = (Vector3.right * h) + (Vector3.forward * v);
            if(1.0f < m_DirVec.magnitude)
                m_DirVec.Normalize();   //�����������

            //ȭ�� �ٱ����� ������ ���ϵ��� ����
            Vector3 a_Pos = Camera.main.WorldToViewportPoint(transform.position);
            if (a_Pos.x < minX)
            {
                a_Pos.x = minX + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.x > maxX)
            {
                a_Pos.x = maxX - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else if (a_Pos.y < minY)
            {
                a_Pos.y = minY + 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            if (a_Pos.y > maxY)
            {
                a_Pos.y = maxY - 0.0001f;
                Vector3 screenOut = Camera.main.ViewportToWorldPoint(a_Pos);
                transform.position = screenOut;
            }
            else
                transform.Translate(m_DirVec * m_MoveSpeed * Time.deltaTime);
        }
    }
    #endregion
    #region ---- ���̽�ƽ �̵�
    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        m_JoyMvLen = a_JoyMvLen;
        if (0.0f < a_JoyMvLen)
        {
            m_JoyMvDir = new Vector3(a_JoyMvDir.x, 0.0f, a_JoyMvDir.y);
            
            //UI ��ǥ�� xy��ǥ���� ĳ������ �̵��� xz������ �����̰� �����Ƿ�
            //UI�� y��ǥ�� ĳ���� �̵��� z�࿡�ٰ� �־��ִ� ��
        }
    }

    public void JoyStickMvUpdate()
    {
        if (h != 0.0f || v != 0.0f)
            return;

        //���̽�ƽ �̵��ڵ�
        if (0.0f < m_JoyMvLen)
        {
            m_DirVec = m_JoyMvDir;
            float a_MvStep = m_MoveSpeed * Time.deltaTime;
            transform.Translate(m_DirVec * m_JoyMvLen * a_MvStep, Space.Self);
        }
    }
    #endregion
    #region ---- ���콺 Ŭ���̵�
    //float m_Tick = 0.0f;
    void MousePickCtrl()    //���콺 Ŭ���� �����ϴ� �Լ�
    {
        //if (0.0f < m_Tick)
        //    m_Tick -= Time.deltaTime;

        //if (m_Tick < 0.0f)
        //{
        //    if (Input.GetMouseButton(0) == true)    //���콺 ���ʹ�ư Ŭ����
        //    {
        //        m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //���� ���콺 ��ġ ��
        //        SetMsPicking(m_PickVec);
        //        m_Tick = 0.1f;

        //    }
        //}

        if (Input.GetMouseButtonDown(0) == true &&
            GameMgr.IsPointerOverUIObject() == false)    //���콺 ���ʹ�ư Ŭ����
        {
            m_PickVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);    //���� ���콺 ��ġ ��
            SetMsPicking(m_PickVec);

            if (m_ClickMark != null)
                m_ClickMark.PlayEff(m_PickVec, this);
        }

    }

    void SetMsPicking(Vector3 a_Pos)
    {
        Vector3 a_CacVec = a_Pos - this.transform.position; //���� ĳ���� ��ġ���� Ÿ�� ��ġ�� ���ϴ� ����
        a_CacVec.y = 0;
        if (a_CacVec.magnitude < 1.0f)  //�ʹ� ª�� �Ÿ��� �̵� �Ƚ�ų��
            return;

        m_bMoveOnOff = true;    //Ŭ������ �̵� ���� ����
        m_DirVec = a_CacVec;    
        m_DirVec.Normalize();   //���� ����
        m_TargetPos = new Vector3(a_Pos.x, transform.position.y, a_Pos.z);  //x�� z�� ���� ��ǥ ���� ����
    }

    void MousePickUpdate()
    {
        if (0.0f < m_JoyMvLen || h != 0.0f || v != 0.0f) //Ű���� �̵� ���̸�, ���̽�ƽ �̵� ���̸�
            m_bMoveOnOff = false;   //���콺 �̵� ���

        if (m_bMoveOnOff == true)
        {
            m_CacStep = Time.deltaTime * m_MoveSpeed;   //�̹� �����ӿ� �����̰� �� �� ����
            Vector3 a_CacEndVec = m_TargetPos - transform.position;
            a_CacEndVec.y = 0.0f;

            if (a_CacEndVec.magnitude <= m_CacStep)  //��ǥ���������� �Ÿ����� ������ ũ�ų� ������ �������� ����
            {
                m_bMoveOnOff = false;
            }
            else
            {
                m_DirVec = a_CacEndVec;
                m_DirVec.Normalize();
                transform.Translate(m_DirVec*m_CacStep, Space.World);
            }
        }

    }
#endregion
    public void ShootFire(Vector3 a_Pos)    //��ǥ������ �Ű������� ����
    {//Ŭ�� �̺�Ʈ�� �߻����� �� �Լ� ȣ��
        GameObject a_Obj = Instantiate(GameMgr.m_BulletPrefab);

        m_CacEndVec = a_Pos - transform.position;   //��ǥ���� - ����ĳ��������
        m_CacEndVec.y = 0.0f;

        BulletCtrl a_BulletSc = a_Obj.GetComponent<BulletCtrl>();
        a_BulletSc.BulletSpawn(transform.position, m_CacEndVec.normalized, m_ShootRange);
    }
}
