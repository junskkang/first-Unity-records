using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroCtrl : MonoBehaviour
{
    //키보드 이동 관련 변수
    float h = 0, v = 0;
    Vector3 moveVec = Vector3.zero;
    float m_CacRotY = 0.0f;
    float m_RotKbSpeed = 100.0f;
    float m_RunVelocity = 5.0f;
    float m_WalkVelocity = 2.0f;
    bool isRun = false;

    //Picking 관련 변수
    Ray m_MouseRay;
    RaycastHit hitInfo;
    LayerMask LayerMask = -1;

    //mouse 이동 계산용 변수
    Vector3 m_TargetPos = Vector3.zero;     //이동할 목표 좌표
    bool m_IsPickMoveOnOff = false;         //마우스 피킹 이동 OnOff
    Vector3 m_MoveDir = Vector3.zero;       //이동할 방향 좌표
    float m_MoveDurTime = 0.0f;             //이동지점까지 소요되는 시간 = 거리/속도
    double m_AddTimeCount = 0.0f;           //누적시간 카운트
    Vector3 m_CacLenVec = Vector3.zero;    //이동 계산용 변수
    Quaternion m_TargetRot = Quaternion.identity;   //회전 계산용 변수
    float m_RotSpeed = 10.0f;

    //조이스틱 이동 관련 변수
    float joyMoveLen = 0.0f;
    Vector3 joyMoveDir = Vector3.zero;

    //애니메이션 관련 변수
    Animator anim;
    AnimState preState = AnimState.idle;
    AnimState curState = AnimState.idle;

    //공격관련 변수
    [HideInInspector] public bool isAttack = false;
    float attackSpeed = 2.0f;

    private void Awake()
    {
        CameraCtrl a_Cam = Camera.main.GetComponent<CameraCtrl>();
        if (a_Cam != null)
            a_Cam.InitCamera(this.gameObject);

        //if(pv.IsMine) 으로 바꿀 예정
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
        h = Input.GetAxisRaw("Horizontal"); //좌우 회전
        v = Input.GetAxisRaw("Vertical");   //전진 이동


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
        //if (v < 0.0f)   //후진 애니메이션이 없어서 후진 막음
        //    v = 0.0f;

        if (h != 0.0f || v != 0.0f)
        {
            ClearMousePickMove();

            //좌우는 회전만 이동은 전진으로만
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

    void MousePicking(Vector3 a_PickVec, GameObject a_PickMon = null)    //마우스 클릭 처리 함수
    {
        a_PickVec.y = transform.position.y;         //목표위치
        Vector3 a_StartPos = transform.position;    //시작위치

        m_CacLenVec = a_PickVec - a_StartPos;
        m_CacLenVec.y = 0.0f;   //Y축 이동 제한

        if (m_CacLenVec.magnitude < 0.5f) return;   //근거리 이동은 제한

        m_TargetPos = a_PickVec;        //최종 이동 위치
        m_IsPickMoveOnOff = true;       //피킹 이동 On/Off

        m_MoveDir = m_CacLenVec.normalized;
        m_MoveDurTime = m_CacLenVec.magnitude / m_RunVelocity; //도착하는데까지 걸리는 시간 = 거리/속도
        m_AddTimeCount = 0.0f;
    }

    void MousePickUpdate()
    {
        if (m_IsPickMoveOnOff)
        {
            m_CacLenVec = m_TargetPos - transform.position;
            m_CacLenVec.y = 0.0f;

            //위에서 계산하였지만 한번 더 계산해주는 이유는
            //모종의 이유로 해당 방향대로 이동하다가 캐릭터의 위치가 바뀌었을경우 (몬스터와 접촉 등)
            //포지션이 밀렸는데 방향은 그대로이기 때문에 최종적으로 도착하는 지점이 달라질 수 있다.
            //고로 한번 더 계산해주어 방향을 재확인 해준다고 생각해주면 좋다.
            m_MoveDir = m_CacLenVec.normalized;

            //캐릭터를 이동방향으로 회전시키는 코드
            if (0.0001f < m_CacLenVec.magnitude)
            {
                m_TargetRot = Quaternion.LookRotation(m_MoveDir);  //해당 벡터에 담긴 방향을 오일러앵글로
                //Debug.Log(m_TargetRot);
                transform.rotation = Quaternion.Slerp(transform.rotation, m_TargetRot, Time.deltaTime * m_RotSpeed);
            }

            m_AddTimeCount += Time.deltaTime;
            if (m_MoveDurTime <= m_AddTimeCount) //목표지점에 도착한 것으로 판정
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


        //마우스 클릭마크 취소
    }

    public void SetJoyStickMv(float a_JoyMvLen, Vector3 a_JoyMvDir)
    {
        joyMoveLen = a_JoyMvLen;
        if (0.0f < a_JoyMvLen)
        {
            //joyMoveDir = new Vector3(a_JoyMvDir.x, 0.0f, a_JoyMvDir.y);

            //카메라가 바라보는 방향으로 항상 이동 시키기 
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

        anim.ResetTrigger(preState.ToString()); //해당 트리거를 취소시키는 옵션

        if (0.0 < crossTime)    //보간시간이 존재하면
        {
            anim.SetTrigger(newState.ToString());
        }
        else
        {
            anim.Play(animName, -1, 0);
            //가운데 인수 -1 : Layer Index
            //마지막 인수 0 : 애니메이션을 처음부터 다시 실행시키겠다는 의미
        }

        preState = newState;
        curState = newState;
    }

    void UpdateAnimState()  //애니메이션 상태 업데이트 메서드
    {
        if ((h != 0.0f || v != 0.0f) || m_IsPickMoveOnOff || 0.0f < joyMoveLen)  //키보드 이동중이거나 마우스 이동 중이면...
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
