using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    //ī�޶� ���� ����
    public Camera followCamera;
    //�̵����� ����
    float hAxis;
    float vAxis;
    public float playerSpeed;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool rDown;         //reload �ٿ� ������
    bool gDown;         //grenade
    bool sDown1;        //swap 1�� ���
    bool sDown2;
    bool sDown3;    

    float jumpPower = 15;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady =true;
    bool isReload;
    bool isBorder;      //�� ���� ������
    bool isDamaged;     //�ǰݽ� ��� ����

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    //�ִϸ��̼� ���� ����
    Animator anim;
    MeshRenderer[] meshRenderers;

    //���� ��� ������Ʈ
    GameObject nearObject;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    Weapon equipWeapon;
    int equipWeaponIdx = -1;

    //���� ����
    float fireDelay;

    //������ ����
    public int ammo;
    public int coin;
    public int health;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;

    public int hasGrenade;
    public int maxHasGrenade;
    public GameObject[] grenades;   //���� ������ ����ź ����
    public GameObject grenadeObj;   //���� ����ź ����

    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady || isReload)
            moveVec = Vector3.zero;

        if(!isBorder)
            transform.position += moveVec * playerSpeed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //Ű���忡 ���� ȸ��
        transform.LookAt(transform.position + moveVec);

        //���ݽ� ���콺�� ���� ȸ��
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);  //��ũ������ ����� ray�� ��� �Լ�
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
            {
                Vector3 nextVec = rayHit.point - transform.position; //���̰� ���� �������� �÷��̾��� ��ġ�� ��
                nextVec.y = 0.0f;       //���̰� �ִ� ������Ʈ ���� ���콺�� �ö��� �� ���� �Ĵٺ��� ���� ����
                //�÷��̾��� ���� ��ġ���� �������� �ش� �������� ȸ���ؾ��ϱ� ������ �Ʒ��� ���� ������ �־��ش�.
                transform.LookAt(transform.position + nextVec);  
            }
        }
    }

    void Jump()
    {
        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Grenade()
    {
        if (hasGrenade == 0) return;

        if (gDown && !isReload && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);  //��ũ������ ����� ray�� ��� �Լ�
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
            {
                Vector3 nextVec = rayHit.point - transform.position; //���̰� ���� �������� �÷��̾��� ��ġ�� ��
                nextVec.y = 20f;

                Vector3 yUp = transform.position;
                yUp.y += 3.0f;      //�������ڸ��� �ٴ��� �����ع����� �ǵ������� ��¦ ���ʿ� �����ǵ��� ��
                GameObject instGrenade = Instantiate(grenadeObj, yUp, transform.rotation);
                Rigidbody rigidGrenade = instGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);      //������ ��
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);   //ȸ�� ��

                hasGrenade--;
                grenades[hasGrenade].SetActive(false);
            }
        }
    }

    void Attack()
    {
        if(equipWeapon == null) return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.UseWeapon();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null) return;

        if (equipWeapon.type == Weapon.Type.Melee) return;

        if (ammo == 0) return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 1f);
        }
    }

    void ReloadOut()
    {
        //�÷��̾ ������ �ִ� źâ(ammo)�� �ƽ��Ƹ𺸴� ���ٸ� �÷��̾ ���� �ִ� ammo��ŭ�� ���ε�
        //�ƴϸ� �ƽ��Ƹ�ŭ ���ε�
        int reAmmo = ammo + equipWeapon.curAmmo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo - equipWeapon.curAmmo;
        equipWeapon.curAmmo += reAmmo;
        ammo -=reAmmo;
        isReload = false;
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isSwap)
        {
            dodgeVec = moveVec;
            playerSpeed *= 2;
            
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.6f);

        }
    }

    void DodgeOut()
    {
        playerSpeed *= 0.5f;
        isDodge = false;
    }
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIdx == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIdx == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIdx == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if((sDown1 ||  sDown2 || sDown3) && !isJump && !isDodge) 
        {
            if(equipWeapon != null)     //����̸� �������� ������
                equipWeapon.gameObject.SetActive(false);   //�̹� ��� �ִ� ��� ������ �װ� ���ּ���

            equipWeaponIdx = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>(); 
            equipWeapon.gameObject.SetActive(true);        //���� ����� ����

            anim.SetTrigger("doSwap");
            isSwap = true;

            Invoke("SwapOut", 0.5f);
        }
    }

    void SwapOut()
    {        
        isSwap = false;
    }
    void Interaction()
    {
        if (iDown && nearObject != null && !isJump && !isDodge)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void FreezeRotation() //�ǵ�ġ ���� �浹�� ���� �÷��̾� ȸ���� �����ϱ� ���� �Լ�
    {
        rigid.angularVelocity = Vector3.zero;   //AugularVelocity : ���� ȸ�� �ӵ�
        
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);  //���� ��ġ, ���� * ����, ��
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //Raycast(������ġ, ����, ����, ���̾��ũ)
        //�ش� ����ĳ��Ʈ�� wall�̶�� ���̾��ũ�� ������ true���� ��ȯ
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item_Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item_Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item_Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item_Type.Grenade:
                    if (hasGrenade == maxHasGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;

                    break;
            }
            Destroy(item.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamaged)
            {
                BulletCtrl enemyBullet = other.GetComponent<BulletCtrl>();
                health -= enemyBullet.damage;
                StartCoroutine(OnDamage());
            }

        }
    }

    IEnumerator OnDamage()
    {
        isDamaged = true;

        foreach (MeshRenderer mesh in meshRenderers)    //�ǰ� ����Ʈ
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1.0f);

        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.material.color = Color.white;
        }
        isDamaged = false;
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Weapon")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Weapon")
            nearObject = null;
    }
}
