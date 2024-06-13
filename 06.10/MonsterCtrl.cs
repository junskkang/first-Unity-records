using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //������ ���� ������ �ִ� Enumerable ���� ����
    public enum MonsterState { idle, trace, attack, die };
    //������ ���� ������ ������ enum ����
    public MonsterState state = MonsterState.idle;

    //��� ������Ʈ
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    Animator anim;

    //���� �����Ÿ�
    public float traceDist = 10.0f;
    //���� �����Ÿ�
    public float attackDist = 2.0f;

    //������ ��� ����
    private bool isDie = false;


    // Start is called before the first frame update
    void Start()
    {
        //������ Transform ������
        monsterTr = this.gameObject.GetComponent<Transform>();
        //���� ��� Player�� Transform�� ������
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgent ������Ʈ�� ������
        nvAgent = gameObject.GetComponent<NavMeshAgent>();
        //�ִϸ����� ������Ʈ ������
        anim = gameObject.GetComponent<Animator>();

        //NavMeshAgent�� �ִ� destination �ɼ��� ���� ���� ����
        //nvAgent.destination = playerTr.position;

        //������ �������� ������ �ൿ���¸� üũ�ϴ� �ڷ�ƾ �Լ� ����
        StartCoroutine(this.CheckMonsterState());

        //������ ���¿� ���� �����ϴ� ��ƾ�� �����ϴ� �ڷ�ƾ �Լ� ����
        StartCoroutine(this.MonsterAction());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //������ ������ �ΰ� ������ ���¸� üũ�Ͽ� monsterState�� ����
    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            //0.2�� ���� ��ٷȴٰ� �������� �Ѿ
            yield return new WaitForSeconds(0.2f);

            //���Ϳ� �÷��̾� ������ �Ÿ� ����
            float dist = Vector3.Distance(playerTr.position, monsterTr.position);

            //ª�� �Ÿ����� if���� �ɸ����� üũ
            if ((dist <= attackDist))    //�÷��̾���� �Ÿ��� ���ݹ��� ���� ���Դ��� üũ
            {
                state = MonsterState.attack;
            }
            else if ((dist <= traceDist))   //�÷��̾���� �Ÿ��� �������� ���� ���Դ��� üũ
            {
                state = MonsterState.trace;                
            }
            else
            {
                state = MonsterState.idle;
            }
        }
    }

    //monsterState���� ���� ���� ���� �Լ�
    IEnumerator MonsterAction()
    {
        while (!isDie) 
        {
            if (isDie)          //�ڷ�ƾ �Լ��� ���������� �ڵ�
                yield break;


            switch (state)
            { 
                case MonsterState.idle:
                    //���� ����
                    nvAgent.isStopped = true;
                    //�߰� �ִϸ��̼� ����
                    anim.SetBool("IsTrace", false);
                    break;

                case MonsterState.trace:
                    //���� ����� ��ġ�� �Ѱ���
                    nvAgent.destination = playerTr.position;
                    //������ �����
                    nvAgent.isStopped = false;
                    //�ִϸ��̼� ���
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
            yield return null;  //�� �������� ���� ���� ��� ���
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
