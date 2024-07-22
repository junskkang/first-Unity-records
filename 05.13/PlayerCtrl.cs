using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    //카메라 관련 변수
    public Camera followCamera;
    //이동관련 변수
    float hAxis;
    float vAxis;
    public float playerSpeed;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool rDown;         //reload 다운 재장전
    bool gDown;         //grenade
    bool sDown1;        //swap 1번 장비
    bool sDown2;
    bool sDown3;    

    float jumpPower = 15;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady =true;
    bool isReload;
    bool isBorder;      //벽 관통 방지용
    bool isDamaged;     //피격시 잠깐 무적

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    //애니메이션 관련 변수
    Animator anim;
    MeshRenderer[] meshRenderers;

    //접근 장비 오브젝트
    GameObject nearObject;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    Weapon equipWeapon;
    int equipWeaponIdx = -1;

    //공격 변수
    float fireDelay;

    //아이템 변수
    public int ammo;
    public int coin;
    public int health;
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;

    public int hasGrenade;
    public int maxHasGrenade;
    public GameObject[] grenades;   //몸을 공전할 수류탄 연결
    public GameObject grenadeObj;   //던질 수류탄 연결

    
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
        //키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);

        //공격시 마우스에 의한 회전
        if (fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);  //스크린에서 월드로 ray를 쏘는 함수
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
            {
                Vector3 nextVec = rayHit.point - transform.position; //레이가 닿은 지점에서 플레이어의 위치를 뺌
                nextVec.y = 0.0f;       //높이가 있는 오브젝트 위에 마우스가 올라갔을 때 높이 쳐다보는 현상 제거
                //플레이어의 현재 위치값을 기준으로 해당 방향으로 회전해야하기 때문에 아래와 같이 변수를 넣어준다.
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
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);  //스크린에서 월드로 ray를 쏘는 함수
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, Mathf.Infinity))
            {
                Vector3 nextVec = rayHit.point - transform.position; //레이가 닿은 지점에서 플레이어의 위치를 뺌
                nextVec.y = 20f;

                Vector3 yUp = transform.position;
                yUp.y += 3.0f;      //생성하자마자 바닥을 감지해버려서 의도적으로 살짝 위쪽에 생성되도록 함
                GameObject instGrenade = Instantiate(grenadeObj, yUp, transform.rotation);
                Rigidbody rigidGrenade = instGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);      //던지는 힘
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);   //회전 힘

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
        //플레이어가 가지고 있는 탄창(ammo)가 맥스아모보다 적다면 플레이어가 갖고 있는 ammo만큼만 리로드
        //아니면 맥스아모만큼 리로드
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
            if(equipWeapon != null)     //빈손이면 실행하지 마세요
                equipWeapon.gameObject.SetActive(false);   //이미 들고 있는 장비가 있으면 그걸 꺼주세요

            equipWeaponIdx = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>(); 
            equipWeapon.gameObject.SetActive(true);        //누른 무기로 스왑

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

    void FreezeRotation() //의도치 않은 충돌로 인한 플레이어 회전을 방지하기 위한 함수
    {
        rigid.angularVelocity = Vector3.zero;   //AugularVelocity : 물리 회전 속도
        
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);  //시작 위치, 방향 * 길이, 색
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        //Raycast(시작위치, 방향, 길이, 레이어마스크)
        //해당 레이캐스트가 wall이라는 레이어마스크에 닿으면 true값을 반환
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

        foreach (MeshRenderer mesh in meshRenderers)    //피격 이펙트
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
