using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class MonsterCtrl : MonoBehaviour
{
    //������ ���� ������ �ִ� Enumerable ���� ����
    public enum MonsterState { idle, trace, attack, rangeAttack, die };

    //������ ���� ���� ������ ������ Enum ����
    public MonsterState monsterState = MonsterState.idle;

    //�ӵ� ����� ���� ���� ������Ʈ�� ������ �Ҵ�
    private Transform monsterTr;
    private Transform playerTr;
    //private NavMeshAgent nvAgent;
    private Animator animator;
    private Rigidbody rigid;

    //���� �����Ÿ�
    public float traceDist = 8.0f;
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

    //���Ÿ� ���ݿ� ���� ����
    bool seePlayer = false;
    public GameObject bulletPrefab;
    float shotSpeed = 3.0f;
    public Transform firePos;
    //������ Ǯ��
    LayerMask layerMask = -1;

        

    // Start is called before the first frame update
    void Awake()
    {
        traceDist = 10.0f;
        attackDist = 1.7f;

        //������ Transform �Ҵ�
        monsterTr = this.gameObject.GetComponent<Transform>();
        rigid = this.gameObject.GetComponent<Rigidbody>();
        //FireCtrl fireCtrl = 
        //coinPrefab = Resources.Load("CoinItem/CoinPrefab") as GameObject;

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

    private void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Default");  //�ǹ� �� ���̾� üũ
    }


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

        Vector3 dir = playerTr.position - monsterTr.position; // - playerTr.position;
        


        if (dir.magnitude <= attackDist) //���� ���� ������ ���Դ��� Ȯ��
        {
            monsterState = MonsterState.attack;
        }
        else if (dir.magnitude <= traceDist && Mathf.Abs(dir.y) <= 5.0f) //���� ���� ������ ���Դ��� Ȯ��
        {
            monsterState = MonsterState.trace;
        }
        //else if (traceDist < dir.magnitude && Mathf.Abs(dir.y) <= 3.0f)
        //{
        //    //Debug.DrawRay(transform.position, dir.normalized * 60, Color.blue);
        //    RaycastHit hit;
        //    seePlayer = Physics.Raycast(transform.position, dir.normalized, out hit, 60.0f);
        //    if (seePlayer)
        //    {
        //        if (!hit.collider.tag.Contains("Player"))
        //        {
        //            monsterState = MonsterState.idle;
        //            return;
        //        }
        //        if (hit.collider.tag.Contains("Player"))
        //        {                    
        //            monsterState = MonsterState.rangeAttack;
        //        }
         
        //    }
        //}
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
            case MonsterState.rangeAttack:
                {
                    float rotSpeed = 6.0f;
                    Vector3 cacDir = playerTr.position - transform.position;
                    cacDir.y = 0.0f;
                    if (0.0f < cacDir.magnitude)
                    {
                        Quaternion targetRot = Quaternion.LookRotation(cacDir.normalized);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed);
                    }

                    //shotSpeed -= Time.deltaTime;
                    //if (shotSpeed < 0.0f)
                    //{
                    //    //Fire(BulletType.E_BULLET, playerTr);
                    //    shotSpeed = 3.0f;
                    //}
                }
                break;


        }

        FireUpdate();
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
            //�Ӹ� �� �ؽ�Ʈ ����
            GameManager.inst.SpawnText(-(int)(coll.gameObject.GetComponent<BulletCtrl>().damage), 
                                            transform.position, Color.red);

            ////���� �Ѿ��� Damage�� ������ ���� hp ����
            //hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            //if(hp <= 0)
            //{
            //    MonsterDie();
            //}

            TakeDamage(coll.gameObject.GetComponent<BulletCtrl>().damage);
            //Bullet ����
            BulletCtrl bulletCtrl = coll.gameObject.GetComponent<BulletCtrl>();
            //�浹�� �Ѿ� ����
            bulletCtrl.PushObjectPool(); //StartCoroutine(bulletCtrl.PushObjectPool(0));


            //IsHit Trigger�� �߻���Ű�� Any State���� gothit�� ���̵�
            //animator.SetTrigger("IsHit");
        }
    }

    void FireUpdate()   //�ֱ������� �Ѿ��� �߻��ϴ� �Լ� ������ Ǯ��
    {
        Vector3 playerPos = playerTr.position;
        playerPos.y += 1.5f;
        Vector3 monsterPos = transform.position;
        monsterPos.y += 1.5f;
        Vector3 cacDir = playerPos - monsterPos;
        float rayUpDownLimit = 3.0f;

        shotSpeed -= Time.deltaTime;
        if (shotSpeed <= 0.0f) 
        {
            shotSpeed = 0.0f;
        }

        if (cacDir.magnitude <= traceDist) return; //�߰ݰŸ� �����̸� ����

        if (!(-rayUpDownLimit <= cacDir.y && cacDir.y < rayUpDownLimit)) return; // -3~ +3������ üũ

        bool isRay = false;
        if (Physics.Raycast(monsterPos, cacDir.normalized, out RaycastHit hit, 100.0f, layerMask))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                isRay = true;
                //Debug.Log(gameObject.name + "�ʸ� ����");
            }
        }

        //���Ϳ��� ���ΰ������� ���� �þ߿� ���ΰ��� ������ ������ ����
        if (!isRay) return;

        //���Ͱ� ���ΰ��� ���� �ٶ󺸵���
        Vector3 lookPlayer = playerTr.position - transform.position;
        lookPlayer.y = 0.0f;    //���͸� �������θ� ȸ����Ű�� ���ؼ� 
        Quaternion targetRot = Quaternion.LookRotation(lookPlayer.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 6.0f);
        //Debug.Log(gameObject.name + "ȸ���Ϸ�");
        //�Ѿ� �߻� ���� ����
        if (shotSpeed <= 0.0f)
        {
            Vector3 startPos = cacDir.normalized;
            firePos.forward = startPos;
            //startPos.y += 1f;
            GameObject go = Instantiate(bulletPrefab, firePos.transform.position, firePos.rotation);
            go.layer = LayerMask.NameToLayer("E_BULLET");
            go.tag = "E_BULLET";
            go.transform.forward = firePos.forward;
            //go.gameObject.SetActive(true);
            shotSpeed = 3.0f;
        }

    }
    void Fire(BulletType type, Transform target)
    {    
        Vector3 dir = target.position - firePos.transform.position;
        dir.y += 1.0f;
        dir.Normalize();
        
        firePos.forward = dir;
        
        GameObject go = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
        go.gameObject.tag = type.ToString();
        //go.transform.forward = firePos.forward;
    }
    public void TakeDamage(int damage = 0)
    {
        if (hp <= 0.0f) return;     //���� ����� ����

        //���� ȿ�� �Լ� ȣ��
        CreateBloodEffect(transform.position);

        hp -= damage;
        if (hp <= 0) 
        {
            hp = 0;
            MonsterDie();
            return;
        }

        //IsHit Trigger�� �߻���Ű�� Any State���� gothit�� ���̵�
        animator.SetTrigger("IsHit");
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
