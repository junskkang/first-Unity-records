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

    //회전 속도 변수
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;

    //아래에 있는 3D 모델의 Animation 컴포넌트에 접근하기 위한 변수
    public Animation _animation;

    //캐릭터 컨트롤러를 이용한 이동, 점프 변수
    CharacterController _characterController;   //캐릭터가 가지고 있는 컴포넌트에 접근하기 위한 변수
    float jumpHeight = 10.0f;
    float yVelocity = 0.0f;
    float gravity = 0.4f;
    bool isJump = false;

    //Player의 생명 변수
    public int hp = 100;
    private int initHp;
    public Image imgHpbar;
    public Text hpText;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        moveSpeed = 6.0f;

        //생명력 초기값 설정
        initHp = hp;

        //hp 텍스트 갱신
        hpText.text = $"{hp} / {initHp}";

        //자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        _animation = GetComponentInChildren<Animation>();

        _characterController = GetComponent<CharacterController>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        _animation.clip = anim.idle;
        _animation.Play();
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

            //캐릭터에 중력이 적용되는 이동함수
            _characterController.SimpleMove(moveDir * moveSpeed);

            //Debug.Log(_characterController.isGrounded);
            Jump();
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
    }

    void Jump()
    {
        if (_characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                yVelocity = jumpHeight;
                isJump = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) && isJump)
            {
                yVelocity += jumpHeight;
                isJump = false;
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
            hp -= 10;

            //체력 UI이미지 갱신
            imgHpbar.fillAmount = (float)hp / (float)initHp;

            //hp 텍스트 갱신
            hpText.text = $"{hp} / {initHp}";

            //Debug.Log("Player HP = " + hp.ToString());

            //Player의 생명이 0이하이면 사망 처리
            if(hp <= 0)
            {
                PlayerDie();
            }
        }

        if (coll.name.Contains("Coin"))
        {
            //점수 획득
            GameManager.inst.DispGold(10);

            //오브젝트 삭제
            Destroy(coll.gameObject);
        }
    }

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
}
