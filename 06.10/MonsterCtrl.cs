using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //몬스터의 상태 정보가 있는 Enumerable 변수 선언
    public enum MonsterState { idle, trace, attack, die };
    //몬스터의 상태 정보를 저장할 enum 변수
    public MonsterState state = MonsterState.idle;

    //대상 컴포턴트
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    Animator anim;

    //추적 사정거리
    public float traceDist = 10.0f;
    //공격 사정거리
    public float attackDist = 2.0f;

    //몬스터의 사망 여부
    private bool isDie = false;


    // Start is called before the first frame update
    void Start()
    {
        //몬스터의 Transform 가져옴
        monsterTr = this.gameObject.GetComponent<Transform>();
        //추적 대상 Player의 Transform을 가져옴
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgent 컴포넌트를 가져옴
        nvAgent = gameObject.GetComponent<NavMeshAgent>();
        //애니메이터 컴포넌트 가져옴
        anim = gameObject.GetComponent<Animator>();

        //NavMeshAgent에 있는 destination 옵션을 통해 추적 시작
        //nvAgent.destination = playerTr.position;

        //일정한 간격으로 몬스터의 행동상태를 체크하는 코루틴 함수 실행
        StartCoroutine(this.CheckMonsterState());

        //몬스터의 상태에 따라 동작하는 루틴을 실행하는 코루틴 함수 실행
        StartCoroutine(this.MonsterAction());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //일정한 간격을 두고 몬스터의 상태를 체크하여 monsterState값 변경
    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            //0.2초 동안 기다렸다가 다음으로 넘어감
            yield return new WaitForSeconds(0.2f);

            //몬스터와 플레이어 사이의 거리 측정
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            //짧은 거리부터 if문에 걸리도록 체크
            if ((dist <= attackDist))    //플레이어와의 거리가 공격범위 내로 들어왔는지 체크
            {
                state = MonsterState.attack;
            }
            else if ((dist <= traceDist))   //플레이어와의 거리가 추적범위 내로 들어왔는지 체크
            {
                state = MonsterState.trace;                
            }
            else
            {
                state = MonsterState.idle;
            }
        }
    }

    //monsterState값에 따른 동작 수행 함수
    IEnumerator MonsterAction()
    {
        while (!isDie) 
        {
            if (isDie)          //코루틴 함수를 빠져나가는 코드
                yield break;


            switch (state)
            { 
                case MonsterState.idle:
                    //추적 중지
                    nvAgent.isStopped = true;
                    //추격 애니메이션 중지
                    anim.SetBool("IsTrace", false);
                    break;

                case MonsterState.trace:
                    //추적 대상의 위치를 넘겨줌
                    nvAgent.destination = playerTr.position;
                    //추적을 재시작
                    nvAgent.isStopped = false;
                    //애니메이션 재생
                    anim.SetBool("IsAttack", false);
                    anim.SetBool("IsTrace", true);
                    
                    break;

                case MonsterState.attack:
                    //transform.LookAt(playerTr.position);

                    Vector3 dir = (playerTr.position - monsterTr.position).normalized;
                    Quaternion quat = Quaternion.LookRotation(dir);
                    transform.transform.rotation = quat;

                    anim.SetBool("IsAttack", true);
                    break;
            }
            yield return null;  //한 프레임이 도는 동안 잠시 대기
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "BULLET")
        {
            anim.SetTrigger("IsHit");
        }
    }
}
