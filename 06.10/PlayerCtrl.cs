using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ŭ������ System.Serializable �̶�� ��Ʈ����Ʈ(Attribute)�� ����ؾ�
//Inspector �信 �����
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

    //�̵� �ӵ� ����
    public float moveSpeed = 0.07f;

    //ȸ�� �ӵ� ����
    public float rotSpeed = 100.0f;
    Vector3 m_CacVec = Vector3.zero;

    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    public Anim anim;

    //�Ʒ��� �ִ� 3D ���� Animation ������Ʈ�� �����ϱ� ���� ����
    public Animation _animation;

    //ĳ���� ��Ʈ�ѷ��� �̿��� �̵�, ���� ����
    CharacterController _characterController;   //ĳ���Ͱ� ������ �ִ� ������Ʈ�� �����ϱ� ���� ����
    float jumpHeight = 10.0f;
    float yVelocity = 0.0f;
    float gravity = 0.4f;
    bool isJump = false;

    //Player�� ���� ����
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

        //����� �ʱⰪ ����
        initHp = hp;

        //hp �ؽ�Ʈ ����
        hpText.text = $"{hp} / {initHp}";

        //�ڽ��� ������ �ִ� Animation ������Ʈ�� ã�ƿ� ������ �Ҵ�
        _animation = GetComponentInChildren<Animation>();

        _characterController = GetComponent<CharacterController>();

        //Animation ������Ʈ�� �ִϸ��̼� Ŭ���� �����ϰ� ����
        _animation.clip = anim.idle;
        _animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //ĳ���� ��� ���� �� ���� �Ұ���
        if (GameManager.GameState == GameState.GameEnd) return;

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //�����¿� �̵� ���� ���� ���
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)
            moveDir.Normalize();

        //Translate(�̵����� * Time.deltaTime * ������ * �ӵ�, ������ǥ)
        //transform.Translate(moveDir * Time.deltaTime * moveSpeed, Space.Self);
        //������ǥ �ɼ��� �⺻���� Space relativeTo = Space.Self


        if (_characterController != null)
        {
            //���͸� ���� ��ǥ�� ���ؿ��� ���� ��ǥ�� �������� ��ȯ�Ѵ�.
            moveDir = transform.TransformDirection(moveDir);

            //ĳ���Ϳ� �߷��� ����Ǵ� �̵��Լ�
            _characterController.SimpleMove(moveDir * moveSpeed);

            //Debug.Log(_characterController.isGrounded);
            Jump();
        }

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameManager.IsPointerOverUIObject() == false)
        {
            //Vector3.up ���� �������� rotSpeed��ŭ�� �ӵ��� ȸ��
            transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X") * 3.0f);
            //transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxisRaw("Mouse X") * 3.0f);
            //m_CacVec = transform.eulerAngles;
            //m_CacVec.y += (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X") * 10.0f);
            //transform.eulerAngles = m_CacVec;
        }

        //Ű���� �Է°��� �������� ������ �ִϸ��̼� ����
        if(v >= 0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runForward.name, 0.3f);
        }
        else if(v <= -0.01f)
        { //���� �ִϸ��̼�
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if(h >= 0.01f)
        { //������ �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if(h <= -0.01f)
        { //���� �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        { //���� idle �ִϸ��̼�
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

    //�浹�� Collider�� IsTrigger �ɼ��� üũ���� �� �߻�
    void OnTriggerEnter(Collider coll)
    {
        //�浹�� Collider�� ������ PUNCH�̸� Player�� HP ����
        if(coll.gameObject.tag == "PUNCH")
        {
            hp -= 10;

            //ü�� UI�̹��� ����
            imgHpbar.fillAmount = (float)hp / (float)initHp;

            //hp �ؽ�Ʈ ����
            hpText.text = $"{hp} / {initHp}";

            //Debug.Log("Player HP = " + hp.ToString());

            //Player�� ������ 0�����̸� ��� ó��
            if(hp <= 0)
            {
                PlayerDie();
            }
        }

        if (coll.name.Contains("Coin"))
        {
            //���� ȹ��
            GameManager.inst.DispGold(10);

            //������Ʈ ����
            Destroy(coll.gameObject);
        }
    }

    //Player�� ��� ó�� ��ƾ
    void PlayerDie()
    {
        Debug.Log("Player Die !!");

        //MONSTER��� Tag�� ���� ��� ���ӿ�����Ʈ�� ã�ƿ�
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //��� ������ OnPlayerDie �Լ��� ���������� ȣ��
        foreach(GameObject monster in monsters)
        {
            //�Ź��
            //���� : Ȥ�ó� �Է��� �Լ��� �������� �ʾƵ� ������ ���� ����
            //       ��� ������ ȣ���ϰ� ������ ����
            //���� : �ҽ��� ������� �� �帧�� Ȯ���ϱⰡ �����..
            monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);

            //�������
            //MonsterCtrl a_MonCtrl = monster.GetComponent<MonsterCtrl>();
            //a_MonCtrl.OnPlayerDie();
        }

        _animation.Stop();  //�ִϸ����� ������Ʈ�� �ִϸ��̼� ���� �Լ�

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
                    //ü�� UI�̹��� ����
                    imgHpbar.fillAmount = (float)hp / (float)initHp;

                    //hp �ؽ�Ʈ ����
                    hpText.text = $"{hp} / {initHp}";
                }
                break;
            case PlayerCharacter.Player2:
                {
                    GameManager.inst.player2.GetComponent<PlayerCtrl>().hp = 
                        GameManager.inst.player1.GetComponent<PlayerCtrl>().hp;
                    //ü�� UI�̹��� ����
                    imgHpbar.fillAmount = (float)hp / (float)initHp;

                    //hp �ؽ�Ʈ ����
                    hpText.text = $"{hp} / {initHp}";
                }

                break;
        }
    }
}
