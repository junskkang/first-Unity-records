using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    //������ ���� ������ �ִ� Enumerable ���� ����
    public enum MonsterState { idle, trace, attack, die };

    //������ ���� ���� ������ ������ Enum ����
    public MonsterState monsterState = MonsterState.idle;

    //�ӵ� ����� ���� ���� ������Ʈ�� ������ �Ҵ�
    private Transform monsterTr;
    private Transform playerTr;
    //private NavMeshAgent nvAgent;
    private Animator animator;
    private Rigidbody rigid;

    //���� �����Ÿ�
    public float traceDist = 10.0f;
    //���� �����Ÿ�
    public float attackDist = 1.0f; //2.0f;

    //������ ��� ����
    private bool isDie = false;

    //���� ȿ�� ������
    public GameObject bloodEffect;
    //���� ��Į ȿ�� ������
    public GameObject bloodDecal;

    //���� ���� ����
    private int hp = 100;

    //���� ��� �� ���� ����
    public GameObject coinPrefab;

    // Start is called before the first frame update
    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.7f;

        //������ Transform �Ҵ�
        monsterTr = this.gameObject.GetComponent<Transform>();
        rigid = this.gameObject.GetComponent<Rigidbody>();
        //coinPrefab = Resources.Load("CoinPrefab") as GameObject;

        //���� ����� Player�� Transform �Ҵ�
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

        //NavMeshAgent ������Ʈ �Ҵ�
        //nvAgent = this.gameObject.GetComponent<NavMeshAgent>();

        ////���� ����� ��ġ�� �����ϸ� �ٷ� ���� ����
        //nvAgent.destination = playerTr.position;

        //Animator ������Ʈ �Ҵ�
        animator = this.gameObject.GetComponent<Animator>();


    }

    //private void OnEnable()
    //{
    //    //������ �������� ������ �ൿ ���¸� üũ�ϴ� �ڷ�ƾ �Լ� ����
    //    StartCoroutine(this.CheckMonsterState());

    //    //������ ���¿� ���� �����ϴ� ��ƾ�� �����ϴ� �ڷ�ƾ �Լ� ����
    //    StartCoroutine(this.MonsterAction());
    //}

    
    void FixedUpdate() //�������� �ӵ��� ������ �ӵ��� ȣ��Ǵ� �Լ�. ĳ���� ���� ������ ������ �� ����
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

        //������ ���
        //if (playerTr.gameObject.activeSelf == false)
        //    playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //FindWithTag() �Լ��� Ư¡�� SetActive�� true ��� ã�´�.

        //���� AI �Լ� ȣ��
        CheckMonStateUpdate();
        MonActionUpdate();

       
    }

#region -- ���� AI �Ϲ� �Լ�
    //������ �������� ������ �ൿ���¸� üũ�ϰ� monsterState �� ����
    float m_AI_Delay = 0.0f;

    void CheckMonStateUpdate()
    {
        if (isDie == true) return;

        //0.1�� �ֱ�� üũ�ϱ� ���� ������ ��� �κ�
        m_AI_Delay -= Time.deltaTime;
        if (0.0f < m_AI_Delay) return;  //0���� Ŭ ��쿡�� �Լ��� ������� �ʵ��� ���Ͻ�Ŵ

        m_AI_Delay = 0.1f;  //0.1�� ��õ �� �Ʒ��� �ڵ���� ����� ��

        //���Ϳ� �÷��̾� ������ �Ÿ� ����
        float dist = Vector3.Distance(playerTr.position, monsterTr.position);
        //�÷��̾ 2���� ���� �� 1���� ���Ͱ� �ʻ� �Ÿ��� �ʵ��� y�� ���� ����
        float yDist = Mathf.Abs(playerTr.position.y - monsterTr.position.y); 
        
        if (dist <= attackDist) //���� ���� ������ ���Դ��� Ȯ��
        {
            monsterState = MonsterState.attack;
        }
        else if (dist <= traceDist && yDist <= 5.0f) //���� ���� ������ ���Դ��� Ȯ��
        {
            monsterState = MonsterState.trace;
        }
        else
        {
            monsterState = MonsterState.idle;   //�Ѵ� �ƴϸ� ������
        }

    }

    //������ ���°��� ���� ������ ������ �����ϴ� �Լ�
    void MonActionUpdate()
    {
        if (isDie == true) return;

        switch (monsterState)
        {
            case MonsterState.idle:
                //Animation�� isTrace ������ false�� ����
                animator.SetBool("IsTrace", false);
                break;
            case MonsterState.trace:
                {
                    //���� �̵� ����
                    float moveVelocity = 2.0f; //��� �ʴ� �̵��ӵ�
                    Vector3 moveDir = playerTr.position - transform.position;
                    moveDir.y = 0.0f;   //�������θ� �̵���Ű�� ���� y�� ����

                    if (0.0f < moveDir.magnitude)
                    {
                        Vector3 stepVec = moveDir.normalized * moveVelocity * Time.deltaTime;
                        transform.Translate(stepVec, Space.World);

                        //�̵������� �ٶ󺸵��� ȸ��
                        float rotSpeed = 7.0f;
                        Quaternion targetRot = Quaternion.LookRotation(moveDir.normalized); //���⺤�͸� ���ʹϿ����� ���
                        transform.rotation = Quaternion.Slerp(transform.rotation,           //�����Լ��� ���� �ڿ������� ȸ����Ű���� ��
                                            targetRot, rotSpeed * Time.deltaTime);
                    }
                    animator.SetBool("IsAttack", false);
                    animator.SetBool("IsTrace", true);
                }
                break;
            case MonsterState.attack:
                {
                    //IsAttack�� true�� ����
                    animator.SetBool("IsAttack", true);

                    //���Ͱ� ���ΰ��� �����ϸ鼭 �ٶ󺸵��� ó��
                    float rotSpeed = 6.0f;
                    Vector3 cacDir = playerTr.position - transform.position;
                    cacDir.y = 0.0f;
                    if (0.0f < cacDir.magnitude)
                    {
                        //AI 0.1�� ���� üũ ������ ���ݰŸ����� �־��� ��� ��ġ ����
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

    #region -- ���� AI �ڷ�ƾ �Լ�
    //������ �������� ������ �ൿ ���¸� üũ�ϰ� monsterState �� ����
    //IEnumerator CheckMonsterState()
    //{
    //    while(!isDie)
    //    {
    //        //0.2�� ���� ��ٷȴٰ� �������� �Ѿ
    //        yield return new WaitForSeconds(0.2f);

    //        //���Ϳ� �÷��̾� ������ �Ÿ� ����
    //        float dist = Vector3.Distance(playerTr.position, monsterTr.position);

    //        if (dist <= attackDist)  //���ݰŸ� ���� �̳��� ���Դ��� Ȯ��
    //        {
    //            monsterState = MonsterState.attack;
    //        }
    //        else if (dist <= traceDist) //�����Ÿ� ���� �̳��� ���Դ��� Ȯ��
    //        {
    //            monsterState = MonsterState.trace; //������ ���¸� �������� ����
    //        }
    //        else
    //        {
    //            monsterState = MonsterState.idle;  //������ ���¸� idle ���� ����
    //        }

    //    }//while(!isDie)
    //}//IEnumerator CheckMonsterState()

    ////������ ���°��� ���� ������ ������ �����ϴ� �Լ�
    //IEnumerator MonsterAction()
    //{
    //    while(!isDie)
    //    {
    //        //if (isDie == true)
    //        //    yield break;  //�ڷ�ƾ �Լ��� ��� ���������� �ڵ�

    //        switch(monsterState)
    //        {
    //            //idle ����
    //            case MonsterState.idle:
    //                //���� ����
    //                //nvAgent.isStopped = true; //<-- nvAgent.Stop();
    //                //Animator�� IsTrace ������ false�� ����
    //                animator.SetBool("IsTrace", false);
    //                break;

    //            //���� ����
    //            case MonsterState.trace:
    //                //���� ����� ��ġ�� �Ѱ���
    //                //nvAgent.destination = playerTr.position;
    //                //������ �����
    //                //nvAgent.isStopped = false; //<--nvAgent.Resume();

    //                //Animator�� IsAttack ������ false�� ����
    //                animator.SetBool("IsAttack", false);
    //                //Animator�� IsTrace �������� true�� ����
    //                animator.SetBool("IsTrace", true);
    //                break;

    //            //���� ����
    //            case MonsterState.attack:
    //                {
    //                    //���� ����
    //                    //nvAgent.isStopped = true; //<-- nvAgent.Stop();
    //                    //IsAttack�� true�� ������ attack State�� ����
    //                    animator.SetBool("IsAttack", true);

    //                    //--- ���Ͱ� ������ �����ϸ鼭 �ٶ� ������ ó��
    //                    float a_RotSpeed = 6.0f;
    //                    Vector3 a_CacDir = playerTr.position - transform.position;
    //                    a_CacDir.y = 0.0f;
    //                    if (0.0f < a_CacDir.magnitude)
    //                    {
    //                        Quaternion a_TargetRot = Quaternion.LookRotation(a_CacDir.normalized);
    //                        transform.rotation = Quaternion.Slerp(
    //                                transform.rotation, a_TargetRot, Time.deltaTime * a_RotSpeed ); 
    //                    }
    //                    //--- ���Ͱ� ������ �����ϸ鼭 �ٶ� ������ ó��
    //                }
    //                break;
    //        }//switch(monsterState)

    //        yield return null; //<-- �� �÷����� ���� ���� ��� ���

    //    }//while(!isDie)
    //}//IEnumerator MonsterAction()
    #endregion
    //Bullet�� �浹 üũ
    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "BULLET")
        {
            //���� ȿ�� �Լ� ȣ��
            CreateBloodEffect(coll.transform.position);

            //���� �Ѿ��� Damage�� ������ ���� hp ����
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if(hp <= 0)
            {
                MonsterDie();
            }

            //Bullet ����
            BulletCtrl bulletCtrl = coll.gameObject.GetComponent<BulletCtrl>();
            //�浹�� �Ѿ� ����
            bulletCtrl.PushObjectPool(); //StartCoroutine(bulletCtrl.PushObjectPool(0));


            //IsHit Trigger�� �߻���Ű�� Any State���� gothit�� ���̵�
            animator.SetTrigger("IsHit");
        }
    }

    //���� ����� ó�� ��ƾ
    void MonsterDie()
    {
        //����� ������ �±׸� ����
        gameObject.tag = "Untagged";

        //��� �ڷ�ƾ�� ����
        StopAllCoroutines();

        isDie = true;
        monsterState = MonsterState.die;
        //nvAgent.isStopped = true;
        animator.SetTrigger("IsDie");

        //�׾��� �� rigidbody �߷� �ȹް� off
        rigid.isKinematic = true;
        //���Ϳ� �߰��� Collider�� ��Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>()) 
        {
            coll.enabled = false;
        }

        //GameManager ���ھ� ���� �Լ� ȣ��
        GameManager.inst.DispScore(50);

        Instantiate(coinPrefab, transform.position, transform.rotation);

        //���� ������Ʈ Ǯ�� ȯ����Ű�� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(this.PushObjectPool());
    }

    IEnumerator PushObjectPool()
    {
        yield return new WaitForSeconds(3.0f);

        //���� ���� �ʱ�ȭ
        isDie = false;
        hp = 100;
        gameObject.tag = "MONSTER";
        monsterState = MonsterState.idle;

        rigid.isKinematic = false;
        //���Ϳ� �߰��� Collider �ٽ� Ȱ��ȭ
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = true;

        foreach (Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = true;
        }

        //���� ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
    void CreateBloodEffect(Vector3 pos)
    {
        //���� ȿ�� ����
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        //��Į ���� ��ġ - �ٴڿ��� ���� �ø� ��ġ ����
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        //��Į�� ȸ������ �������� ����
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        //��Į ������ ����
        GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        //��Į�� ũ�⵵ �ұ�Ģ������ ��Ÿ���Բ� ������ ����
        float scale = Random.Range(1.5f, 3.5f);
        blood2.transform.localScale = Vector3.one * scale;

        //5�� �Ŀ� ����ȿ�� �������� ����
        Destroy(blood2, 5.0f);

    }//void CreateBloodEffect(Vector3 pos)

    //�÷��̾ ������� �� ����Ǵ� �Լ�
    void OnPlayerDie()
    {
        //������ ���¸� üũ�ϴ� �ڷ�ƾ �Լ��� ��� ������Ŵ
        StopAllCoroutines();
        //������ �����ϰ� �ִϸ��̼��� ����
        //nvAgent.isStopped = true;  //<-- nvAgent.Stop();

        //���� ���͵� �ڷ�ƾ ����� �ϰ�, �׺���̼ǸŽõ� ����� �ϴ� ���� ����
        if (isDie == false)  //�ٸ� ���ߴ� �ִϸ��̼Ǹ� ���۾��ϰ� �״�� �׾��ֵ���
            animator.SetTrigger("IsPlayerDie");
    }
}
