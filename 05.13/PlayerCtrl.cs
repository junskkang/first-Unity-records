using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    //이동관련 변수
    float hAxis;
    float vAxis;
    public float playerSpeed;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool sDown1;        //swap 1번 장비
    bool sDown2;
    bool sDown3;    

    float jumpPower = 15;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady =true;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    //애니메이션 관련 변수
    Animator anim;

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
    public GameObject[] grenades;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
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
        Attack();
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
        fDown = Input.GetButtonDown("Fire1");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;

        transform.position += moveVec * playerSpeed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
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

    void Attack()
    {
        if(equipWeapon == null) return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.UseWeapon();
            anim.SetTrigger("doSwing");
            fireDelay = 0;
        }
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
