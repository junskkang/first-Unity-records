using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroCtrl : MonoBehaviour
{
    //�̵����� ����
    float hAxis;
    float vAxis;
    float moveSpeed = 8.0f;
    Vector3 moveVec;

    //ü�¹ٰ��� ����
    public Image HpBar;
    public float curHp = 100.0f;
    public float maxHp = 100.0f;

    //�Ѿ� ���� ���� ����
    float attackSpeed = 0.3f;
    float attackTick = 0.0f;
    float attackRange = 40.0f;
    bool isAttack;
    public GameObject bulletPrefab; 


        
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        PlayerHUD();
        Attack();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        isAttack = Input.GetMouseButton(0);
    }


    void Move()
    {
        moveVec = new Vector3(hAxis, vAxis, 0).normalized;

        if (hAxis != 0 || vAxis != 0) 
            transform.position += moveVec * moveSpeed * Time.deltaTime;

        #region ȭ��� �̵�����
        //ȭ�� �� �̵�����
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);

        //if (pos.x < 0.02f) pos.x = 0.02f;
        //if (0.98f < pos.x) pos.x = 0.98f;
        //if (pos.y < 0.1f) pos.y = 0.1f;
        //if (0.9f < pos.y) pos.y = 0.9f;

        pos.x = Mathf.Clamp(pos.x, 0.03f, 0.97f);
        pos.y = Mathf.Clamp(pos.y, 0.1f, 0.93f);

        Vector3 posLimit = Camera.main.ViewportToWorldPoint(pos);
        posLimit.z = transform.position.z;
        transform.position = posLimit;
        #endregion
    }

    void PlayerHUD()
    {
        //ü�¹� ������ UI
        if (curHp <= 0.0f)
            return;
        else        
        {
            if (HpBar != null)
                HpBar.fillAmount = curHp / maxHp;
        }
    }

    void Attack()
    {
        if (isAttack == false) return;

        attackTick -= Time.deltaTime;

        if (attackTick <= 0.0f)
        {
            BulletFire();
            attackTick = attackSpeed;
        }
    }

    void BulletFire()
    {
        //GameObject obj = Resources.Load("Bullet") as GameObject;
        GameObject obj = Instantiate(bulletPrefab);
        BulletCtrl bulletCtrl = obj.GetComponent<BulletCtrl>();
        bulletCtrl.BulletFire(transform.position);
    }
}
