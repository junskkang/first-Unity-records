using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//클래스에 System.Serializable 이라는 어트리뷰트(Attribute)를 명시해야
//Inspector 뷰에 노출됨
[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;
}

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    //이동 속도 변수
    public float moveSpeed = 0.07f;

    //캐릭터 컨트롤러를 이용한 이동, 점프 변수
    CharacterController _characterController;   //캐릭터가 가지고 있는 컴포넌트에 접근하기 위한 변수
    float jumpHeight = 10.0f;
    float yVelocity = 0.0f;
    float gravity = 0.4f;
    bool canDJump = false;

    float gReturnTime = 0.69f;  //jumpPower를 다 깎아 먹고 velocityY에 도달하는 데에 걸리는 시간
    //점프 감도를 좋게 하기위해서 일정한 속도로 떨어지도록 함. 프레임에 관계없이 deltaTime으로 감소
    float gravitySpeed = 36.2f; //
    float velocityY = -12.0f;   //중력 최대치. 끌어내리는 힘
    float jumpPower = 10.0f;    //점프시 뛰어 오르는 힘

    //회전 속도 변수
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;

    //아래에 있는 3D 모델의 Animation 컴포넌트에 접근하기 위한 변수
    public Animation _animation;

    //사운드를 위한 변수
    public AudioClip CoinSfx;
    public AudioClip DiamondSfy;
    AudioSource _audioSource;

    //Player의 생명 변수
    public int hp = 100;
    private int initHp;
    public Image imgHpbar;
    public Text hpText;

    //스킬 사용을 위한 컴포넌트 추가
    public FireCtrl fireCtrl;
    float shieldDuration = 20.0f;
    float shieldOnTime = 0.0f;
    public GameObject shieldObj = null;

    //피격 이펙트 변수
    public GameObject bloodEffect;
    public GameObject bloodDecal;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        moveSpeed = 6.0f;
        gravitySpeed = (jumpPower + (-velocityY)) / gReturnTime;
        //gReturnTime초 만에 jumpPower를 다 깎아 먹고 velocityY에 도달하는 데에 걸리는 시간
        //생명력 초기값 설정
        initHp = hp;

        //hp 텍스트 갱신
        hpText.text = $"{hp} / {initHp}";

        //자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        _animation = GetComponentInChildren<Animation>();
        
        _characterController = GetComponent<CharacterController>();

        if (fireCtrl == null)
            fireCtrl = GetComponent<FireCtrl>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        _animation.clip = anim.idle;
        _animation.Play();

        //플레이어에도 AudioSource가 붙어있기 때문에 정확히 메시에 붙어있는 오디오 소스를 불러오기 위해
        //대상을 특정하고 거기의 오디오 소스를 불러옴
        Transform playerMesh = transform.Find("PlayerModel");
        if (playerMesh != null)
            _audioSource = playerMesh.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //캐릭터 사망 상태 시 조작 불가능
        if (GameManager.GameState == GameState.GameEnd) return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        //Translate(이동방향 * Time.deltaTime * 변위값 * 속도, 기준좌표)
        //transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        //기준좌표 옵션의 기본값은 Space relativeTo = Space.Self


        if (_characterController != null)
        {
            //벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 변환한다.
            moveDir = transform.TransformDirection(moveDir);
            moveDir = moveDir * moveSpeed;  //변수에 이동속도까지 포함

            if (_characterController.isGrounded)    //점프
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    velocityY = jumpPower;
                    canDJump = true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space) && canDJump)
                {
                    //만약 어느 타이밍에 점프를 다시 눌러도 같은 높이만큼 더블점프를 하게 하고 싶다면
                    //velocityY = 0으로 해놓고 jumpPower를 더해주도록 하자.
                    //현재 상태에서는 타이밍을 두고 더블점프를 하면 두번째 점프는 낮은 점프밖에 되지 않음
                    velocityY += jumpPower;
                    canDJump = false;
                }

            }


            if (-12.0f < velocityY)
                velocityY -= gravitySpeed * Time.deltaTime;

            moveDir.y = velocityY;

            _characterController.Move(moveDir * Time.deltaTime);
            ////캐릭터에 중력이 적용되는 이동함수. Project Setting > Physics에 있는 중력값의 영향
            //_characterController.SimpleMove(moveDir * moveSpeed);
        }

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameManager.IsPointerOverUIObject() == false)
        {
            //Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
            transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);
            //transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxisRaw("Mouse X") * 3.0f);
            //m_CacVec = transform.eulerAngles;
            //m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X") * 10.0f);
            //transform.eulerAngles = m_CacVec;
        }

        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        if(v >= 0.01f)
        { //전진 애니메이션
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if(v <= -0.01f)
        { //후진 애니메이션
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if(h >= 0.01f)
        { //오른쪽 이동 애니메이션
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if(h <= -0.01f)
        { //왼쪽 이동 애니메이션
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        { //정지 idle 애니메이션
            _animation.CrossFade(anim.idle.name, 0.3f);
        }

        SkillUpdate();

    }

    void Jump()
    {
        if (_characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                yVelocity = jumpHeight;
                canDJump = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && canDJump)
            {
                yVelocity += jumpHeight;
                canDJump = false;
            }

            yVelocity -= gravity;
        }

        _characterController.Move(new Vector3(0, yVelocity, 0) * Time.deltaTime);

    }

    //충돌한 Collider의 IsTrigger 옵션이 체크됐을 때 발생
    void OnTriggerEnter(Collider coll)
    {
        //충돌한 Collider가 몬스터의 PUNCH이면 Player의 HP 차감
        if(coll.gameObject.tag == "PUNCH")
        {
            if (0.0f < shieldOnTime) return; //쉴드 발동 중이면 리턴

            if (hp <= 0.0f) return;     //이미 사망중
            TakeDamage(10);
        }

        if (coll.name.Contains("Coin"))
        {
            int cacGold = 10;   //층 올라갈수록 보상 증가를 위해 변수로 사용

            //골드 획득
            GameManager.inst.DispGold(cacGold);

            if (_audioSource != null && CoinSfx != null)
                _audioSource.PlayOneShot(CoinSfx, 0.3f);

            //오브젝트 삭제
            Destroy(coll.gameObject);
        }
    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "E_BULLET")
        {

            TakeDamage(coll.gameObject.GetComponent<BulletCtrl>().damage/2);
            //Bullet 삭제
            Destroy(coll.gameObject);

        }
    }

    void TakeDamage(int damage = 10)
    {
        if (hp <= 0.0f) return;

        hp -= damage;

        CreateBloodEffect(transform.position);

        //체력 UI이미지 갱신
        imgHpbar.fillAmount = (float)hp / (float)initHp;

        //hp 텍스트 갱신
        hpText.text = $"{hp} / {initHp}";

        //Debug.Log("Player HP = " + hp.ToString());

        //Player의 생명이 0이하이면 사망 처리
        if (hp <= 0)
        {
            PlayerDie();
        }
    }

    void CreateBloodEffect(Vector3 pos)
    {
        //혈흔 효과 생성
        pos.y += 1.5f;
        GameObject blood1 = (GameObject)Instantiate(bloodEffect, pos, Quaternion.identity);
        blood1.GetComponent<ParticleSystem>().Play();
        Destroy(blood1, 3.0f);

        ////데칼 생성 위치 - 바닥에서 조금 올린 위치 산출
        //Vector3 decalPos = transform.position + (Vector3.up * 0.05f);
        ////데칼의 회전값을 무작위로 설정
        //Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));

        ////데칼 프리팹 생성
        //GameObject blood2 = (GameObject)Instantiate(bloodDecal, decalPos, decalRot);
        ////데칼의 크기도 불규칙적으로 나타나게끔 스케일 조정
        //float scale = Random.Range(1.5f, 3.5f);
        //blood2.transform.localScale = Vector3.one * scale;

        ////5초 후에 혈흔효과 프리팹을 삭제
        //Destroy(blood2, 5.0f);

    }//void CreateBloodEffect(Vector3 pos)
    //Player의 사망 처리 루틴
    void PlayerDie()
    {
        Debug.Log("Player Die !!");

        //MONSTER라는 Tag를 가진 모든 게임오브젝트를 찾아옴
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //모든 몬스터의 OnPlayerDie 함수를 순차적으로 호출
        foreach(GameObject monster in monsters)
        {
            //신방식
            //장점 : 혹시나 입력한 함수가 존재하지 않아도 에러가 나지 않음
            //       고로 있으면 호출하고 없으면 말고
            //단점 : 소스를 디버깅할 때 흐름을 확인하기가 어려워..
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);

            //기존방식
            //MonsterCtrl a_MonCtrl = monster.GetComponent<MonsterCtrl>();
            //a_MonCtrl.OnPlayerDie();
        }

        _animation.Stop();  //애니메이터 컴포넌트의 애니메이션 중지 함수

        GameManager.GameState = GameState.GameEnd;
        GameManager.inst.isGameOver = true;

        StartCoroutine(GameManager.inst.GameOver());
    }

    public void CharacterChange()
    {
        switch (GameManager.playerCharacter)
        {
            case PlayerCharacter.Player1:
                {
                    GameManager.inst.player1.GetComponent<PlayerCtrl>().hp = 
                        GameManager.inst.player2.GetComponent<PlayerCtrl>().hp;
                    //체력 UI이미지 갱신
                    imgHpbar.fillAmount = (float)hp / (float)initHp;

                    //hp 텍스트 갱신
                    hpText.text = $"{hp} / {initHp}";
                }
                break;
            case PlayerCharacter.Player2:
                {
                    GameManager.inst.player2.GetComponent<PlayerCtrl>().hp = 
                        GameManager.inst.player1.GetComponent<PlayerCtrl>().hp;
                    //체력 UI이미지 갱신
                    imgHpbar.fillAmount = (float)hp / (float)initHp;

                    //hp 텍스트 갱신
                    hpText.text = $"{hp} / {initHp}";
                }

                break;
        }
    }

    void SkillUpdate()
    {
        //쉴드 상태 업데이트
        if (0.0f < shieldOnTime)
        {
            shieldOnTime -= Time.deltaTime;
            if (shieldObj != null && !shieldObj.activeSelf)
                shieldObj.SetActive(true);
        }
        else
        {
            if (shieldObj != null && shieldObj.activeSelf)
                shieldObj.SetActive(false);
        }
    }
    public void UseSkill_Item(SkillType skillType)
    {
        if (GameManager.GameState == GameState.GameEnd) return; 
        
        if (skillType == SkillType.Skill_0)     //체력 회복 스킬
        {
            GameManager.inst.SpawnText((int)(initHp * 0.3f), transform.position, Color.cyan);

            hp += (int)(initHp * 0.3f);

            if (initHp < hp)
                hp = initHp;

            if (imgHpbar != null)
                imgHpbar.fillAmount = hp / (float)initHp;

            //hp 텍스트 갱신
            hpText.text = $"{hp} / {initHp}";
        }
        else if (skillType == SkillType.Skill_1)    //수류탄 투척 스킬
        {
            fireCtrl.FireGrenade();
        }
        else if (skillType == SkillType.Skill_2)    //쉴드 생성 스킬
        {
            if (0.0f < shieldOnTime) return; //발동 중이면 리턴

            shieldOnTime = shieldDuration;

            //쿨타임 발동
            GameManager.inst.SkillTimeMethod(shieldOnTime, shieldDuration);
        }

        int skillIdx = (int)skillType;
        GlobalValue.g_SkillCount[skillIdx]--;
        string a_MkKey = "SkItem_" + skillIdx.ToString();
        PlayerPrefs.SetInt(a_MkKey, GlobalValue.g_SkillCount[skillIdx]);
    }
}
