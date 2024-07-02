using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터의 상태 정보가 있는 Enumerable 변수 선언
    public enum MonsterState { idle, trace, attack, die };

    //몬스터의 현재 상태 정보를 저장할 Enum 변수
    public MonsterState monsterState = MonsterState.idle;

    //속도 향상을 위해 각종 컴포넌트를 변수에 할당
    private Transform monsterTr;
    private Transform playerTr;
    //private NavMeshAgent nvAgent;
    private Animator animator;
    private Rigidbody rigid;

    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 1.0f; //2.0f;

    //몬스터의 사망 여부
    private bool isDie = false;

    //혈흔 효과 프리팹
    public GameObject bloodEffect;
    //혈흔 데칼 효과 프리팹
    public GameObject bloodDecal;

    //몬스터 생명 변수
    private int hp = 100;

    //몬스터 사망 시 코인 스폰
    public GameObject coinPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.7f;

        //몬스터의 Transform 할당
        monsterTr = this.gameObject.GetComponent<Transform>();
        rigid = this.gameObject.GetComponent<Rigidbody>();
        //coinPrefab = Resources.Load("CoinPrefab") as GameObject;

        //추적 대상인 Player의 Transform 할당
        //playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        switch (GameManager.playerCharacter)
        {
            case PlayerCharacter.Player1:
                playerTr = GameManager.inst.player1.transform;
                break;
            case PlayerCharacter.Player2:
                playerTr = GameManager.inst.player2.transform;
                break;
        }

        //NavMeshAgent 컴포넌트 할당
        //nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        ////추적 대상의 위치를 설정하면 바로 추적 시작
        //nvAgent.destination = playerTr.position;

        //Animator 컴포넌트 할당
        animator = this.gameObject.GetComponent<Animator>();


    }

    //private void OnEnable()
    //{
    //    //일정한 간젹으로 몬스터의 행동 상태를 체크하는 코루틴 함수 실행
    //    StartCoroutine(this.CheckMonsterState());

    //    //몬스터의 상태에 따라 동작하는 루틴을 싱행하는 코루틴 함수 실행
    //    StartCoroutine(this.MonsterAction());
    //}

    
    void FixedUpdate() //물리연산 속도와 동일한 속도로 호출되는 함수. 캐릭터 떨림 현상을 보완할 수 있음
    {
        switch (GameManager.playerCharacter)
        {
            case PlayerCharacter.Player1:
                playerTr = GameManager.inst.player1.transform;
                break;
            case PlayerCharacter.Player2:
                playerTr = GameManager.inst.player2.transform;
                break;
        }

        //선생님 방법
        //if (playerTr.gameObject.activeSelf == false)
        //    playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //FindWithTag() 함수의 특징은 SetActive가 true 대상만 찾는다.

        //몬스터 AI 함수 호출
        CheckMonStateUpdate();
        MonActionUpdate();

       
    }

#region -- 몬스터 AI 일반 함수
    //일정한 간격으로 몬스터의 행동상태를 체크하고 monsterState 값 변경
    float m_AI_Delay = 0.0f;

    void CheckMonStateUpdate()
    {
        if (isDie == true) return;

        //0.1초 주기로 체크하기 위한 딜레이 계산 부분
        m_AI_Delay -= Time.deltaTime;
        if (0.0f < m_AI_Delay) return;  //0보다 클 경우에는 함수가 실행되지 않도록 리턴시킴

        m_AI_Delay = 0.1f;  //0.1초 충천 후 아래의 코드들이 실행될 것

        //몬스터와 플레이어 사이의 거리 측정
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        //플레이어가 2층에 있을 때 1층의 몬스터가 뽈뽈 거리지 않도록 y값 따로 측정
        float yDist = Mathf.Abs(playerTr.position.y - monsterTr.position.y); 
        
        if (dist <= attackDist) //공격 범위 안으로 들어왔는지 확인
        {
            monsterState = MonsterState.attack;
        }
        else if (dist <= traceDist && yDist <= 5.0f) //추적 범위 안으로 들어왔는지 확인
        {
            monsterState = MonsterState.trace;
        }
        else
        {
            monsterState = MonsterState.idle;   //둘다 아니면 대기상태
        }

    }

    //몬스터의 상태값에 따라 적절한 동작을 수행하는 함수
    void MonActionUpdate()
    {
        if (isDie == true) return;

        switch (monsterState)
        {
            case MonsterState.idle:
                //Animation의 isTrace 변수를 false로 설정
                animator.SetBool("IsTrace", false);
                break;
            case MonsterState.trace:
                {
                    //추적 이동 구현
                    float moveVelocity = 2.0f; //평면 초당 이동속도
                    Vector3 moveDir = playerTr.position - transform.position;
                    moveDir.y = 0.0f;   //수평으로만 이동시키기 위해 y값 없앰

                    if (0.0f < moveDir.magnitude)
                    {
                        Vector3 stepVec = moveDir.normalized * moveVelocity * Time.deltaTime;
                        transform.Translate(stepVec, Space.World);

                        //이동방향을 바라보도록 회전
                        float rotSpeed = 7.0f;
                        Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized); //방향벡터를 쿼터니온으로 담고
                        transform.rotation = Quaternion.Slerp(transform.rotation,           //보간함수를 통해 자연스럽게 회전시키도록 함
                                            targetRot, rotSpeed * Time.deltaTime);
                    }
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                }
                break;
            case MonsterState.attack:
                {
                    //IsAttack을 true로 설정
                    animator.SetBool("IsAttack", true);

                    //몬스터가 주인공을 공격하면서 바라보도록 처리
                    float rotSpeed = 6.0f;
                    Vector3 cacDir = playerTr.position - transform.position;
                    cacDir.y = 0.0f;
                    if (0.0f < cacDir.magnitude)
                    {
                        //AI 0.1초 간격 체크 때문에 공격거리에서 멀어진 경우 위치 보정
                        if (attackDist < cacDir.magnitude)
                        {
                            float moveVelocity = 2.0f;
                            Vector3 stepVec = cacDir.normalized * moveVelocity * Time.deltaTime;
                            transform.Translate(stepVec, Space.World);
                        }

                        Quaternion targetRot = Quaternion.LookRotation(cacDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
                    }
                }
                break;


        }
    }

    #endregion

    #region -- 몬스터 AI 코루틴 함수
    //일정한 간격으로 몬스터의 행동 상태를 체크하고 monsterState 값 변경
    //IEnumerator CheckMonsterState()
    //{
    //    while(!isDie)
    //    {
    //        //0.2초 동안 기다렸다가 다음으로 넘어감
    //        yield return new WaitForSeconds(0.2f);

    //        //몬스터와 플레이어 사이의 거리 측정
    //        float dist = Vector3.Distance(playerTr.position, monsterTr.position);

    //        if (dist <= attackDist)  //공격거리 범위 이내로 들어왔는지 확인
    //        {
    //            monsterState = MonsterState.attack;
    //        }
    //        else if (dist <= traceDist) //추적거리 범위 이내로 들어왔는지 확인
    //        {
    //            monsterState = MonsterState.trace; //몬스터의 상태를 추적으로 설정
    //        }
    //        else
    //        {
    //            monsterState = MonsterState.idle;  //몬스터의 상태를 idle 모드로 설정
    //        }

    //    }//while(!isDie)
    //}//IEnumerator CheckMonsterState()

    ////몬스터의 상태값에 따라 적절한 동작을 수행하는 함수
    //IEnumerator MonsterAction()
    //{
    //    while(!isDie)
    //    {
    //        //if (isDie == true)
    //        //    yield break;  //코루틴 함수를 즉시 빠져나가는 코드

    //        switch(monsterState)
    //        {
    //            //idle 상태
    //            case MonsterState.idle:
    //                //추적 중지
    //                //nvAgent.isStopped = true; //<-- nvAgent.Stop();
    //                //Animator의 IsTrace 변수를 false로 설정
    //                animator.SetBool("IsTrace", false);
    //                break;

    //            //추적 상태
    //            case MonsterState.trace:
    //                //추적 대상의 위치를 넘겨줌
    //                //nvAgent.destination = playerTr.position;
    //                //추적을 재시작
    //                //nvAgent.isStopped = false; //<--nvAgent.Resume();

    //                //Animator의 IsAttack 변수를 false로 설정
    //                animator.SetBool("IsAttack", false);
    //                //Animator의 IsTrace 변수값을 true로 설정
    //                animator.SetBool("IsTrace", true);
    //                break;

    //            //공격 상태
    //            case MonsterState.attack:
    //                {
    //                    //추적 중지
    //                    //nvAgent.isStopped = true; //<-- nvAgent.Stop();
    //                    //IsAttack을 true로 설정해 attack State로 전이
    //                    animator.SetBool("IsAttack", true);

    //                    //--- 몬스터가 주인을 공격하면서 바라 보도록 처리
    //                    float a_RotSpeed = 6.0f;
    //                    Vector3 a_CacDir = playerTr.position - transform.position;
    //                    a_CacDir.y = 0.0f;
    //                    if (0.0f < a_CacDir.magnitude)
    //                    {
    //                        Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
    //                        transform.rotation = Quaternion.Slerp(
    //                                transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed ); 
    //                    }
    //                    //--- 몬스터가 주인을 공격하면서 바라 보도록 처리
    //                }
    //                break;
    //        }//switch(monsterState)

    //        yield return null; //<-- 한 플레임이 도는 동안 잠시 대기

    //    }//while(!isDie)
    //}//IEnumerator MonsterAction()
    #endregion
    //Bullet과 충돌 체크
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            //혈흔 효과 함수 호출
            CreateBloodEffect(coll.transform.position);

            //맞은 총알의 Damage를 추출해 몬스터 hp 차감
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if(hp <= 0)
            {
                MonsterDie();
            }

            //Bullet 삭제
            BulletCtrl bulletCtrl = coll.gameObject.GetComponent<BulletCtrl>();
            //충돌한 총알 제거
            bulletCtrl.PushObjectPool(); //StartCoroutine(bulletCtrl.PushObjectPool(0));


            //IsHit Trigger를 발생시키면 Any State에서 gothit로 전이됨
            animator.SetTrigger("IsHit");
        }
    }

    //몬스터 사망시 처리 루틴
    void MonsterDie()
    {
        //사망한 몬스터의 태그를 변경
        gameObject.tag = "Untagged";

        //모든 코루틴을 정지
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        //nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        //죽었을 때 rigidbody 중력 안받게 off
        rigid.isKinematic = true;
        //몬스터에 추가된 Collider를 비활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>()) 
        {
            coll.enabled = false;
        }

        //GameManager 스코어 누적 함수 호출
        GameManager.inst.DispScore(50);

        Instantiate(coinPrefab, transform.position, transform.rotation);

        //몬스터 오브젝트 풀로 환원시키는 코루틴 함수 호출
        StartCoroutine(this.PushObjectPool());
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //각종 변수 초기화
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        rigid.isKinematic = false;
        //몬스터에 추가된 Collider 다시 활성화
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //몬스터 비활성화
        gameObject.SetActive(false);
    }
    void CreateBloodEffect(Vector3 pos)
    {
        //혈흔 효과 생성
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        //데칼 생성 위치 - 바닥에서 조금 올린 위치 산출
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //데칼의 회전값을 무작위로 설정
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //데칼 프리팹 생성
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //데칼의 크기도 불규칙적으로 나타나게끔 스케일 조정
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        //5초 후에 혈흔효과 프리팹을 삭제
        Destroy(blood2, 5.0f);

    }//void CreateBloodEffect(Vector3 pos)

    //플레이어가 사망했을 때 실행되는 함수
    void OnPlayerDie()
    {
        //몬스터의 상태를 체크하는 코루틴 함수를 모두 정지시킴
        StopAllCoroutines();
        //추적을 정지하고 애니메이션을 수행
        //nvAgent.isStopped = true;  //<-- nvAgent.Stop();

        //죽은 몬스터도 코루틴 멈춰야 하고, 네비게이션매시도 멈춰야 하는 것이 맞음
        if (isDie == false)  //다만 춤추는 애니메이션만 동작안하고 그대로 죽어있도록
            animator.SetTrigger("IsPlayerDie");
    }
}
