using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //������ ���̰��� �˾ƿ��� ���� ����
    public Terrain m_RefMap = null;

    //UI���� ����
    int hp = 3;
    public Image[] LifeImg;
    public Text timerText;
    float m_Timer = 60.0f;
    public Text scoreText;
    public static int score = 0;

    //���̶� ���� ����
    PlayerCtrl playerCtrl;
    public GameObject MummyRoot;
    float span = 1.0f;    //���̶� �����ֱ�
    float delta = 0.0f;   //���̶� ���� �ֱ� ���� ����
    float m_MvSpeedCtrl = 13.0f;  //��ü ���̶� �̵� �ӵ��� �����ϱ� ���� ����

    //GameScene �ȿ����� ����Ǵ� �̱��� ����
    public static GameManager Inst = null;

    //���� ���� ����
    public GameObject[] AnimalArr;
    public Transform AnimalGroup;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        AnimalRandGen();  //���� ��

        score = 0; //static���� �ʱ�ȭ
        Time.timeScale = 1.0f;

        playerCtrl = GameObject.FindObjectOfType<PlayerCtrl>();
    }

    
    void Update()
    {
        if (PlayerCtrl.state == Moving.Stop)
            MummyGenerator();

        UIUpdate();
    }

    void MummyGenerator()  //ĳ���Ͱ� ������ �� �� �տ� �����ϰ� ���̶� �����ϴ� �Լ�
    {
        //���̵� ����
        //���̶� �̼� ����
        m_MvSpeedCtrl += (Time.deltaTime * 0.1f);
        if (35.0f < m_MvSpeedCtrl)
            m_MvSpeedCtrl = 35.0f;
        //���� �ֱ� ����
        span -= (Time.deltaTime * 0.03f);
        if (span < 0.2f)
            span = 0.2f;

        this.delta += Time.deltaTime;
        if (span < delta)
        {
            delta = 0.0f;

            //z��ǥ ������ 50���� ������ �� ��ǥ ����
            Vector3 CamForw = Camera.main.transform.forward;
            CamForw.y = 0.0f;
            CamForw.Normalize();                   //CamForw�� �� ��ü�� Normalize�� ������ �ٲ����
            //Vector3 a_Norm = CamForw.normalized;   //CamForw�� �� ��ü�� ������ �ʾ�. ����� ���� �ٸ� ���� �־���
            //���⺤�Ϳ� 50~51m �� ������ ������ ��������
            CamForw = CamForw * (float)Random.Range(50.0f, 52.0f);

            //x��ǥ ������ -36~ 35������ ���� ��ǥ ����
            Vector3 CacX = Camera.main.transform.right;
            CacX.y = 0.0f;
            CacX.Normalize();
            CacX = CacX * Random.Range(-36.0f, 36.0f);

            //x��ǥ, z��ǥ�� ��ȭ���� �����Ų �ν��Ͻ� ����
            Vector3 SpPos = Camera.main.transform.position + CamForw + CacX;
            SpPos.y = 0.0f;
            GameObject go = Instantiate(MummyRoot);
            go.transform.position = SpPos;
            go.GetComponent<MummyCtrl>().m_MoveVelocity = m_MvSpeedCtrl;
            float ran = Random.Range(0.3f, 2.0f);
            go.transform.localScale = new Vector3(ran, ran, ran);
        }
    }

    public void DecreaseHp()
    {
        hp--;
        if (hp < 0)
            hp = 0;

        for (int i = 0; i < LifeImg.Length; i++)
        {
            if (i < hp)
                LifeImg[i].gameObject.SetActive(true);
            else
                LifeImg[i].gameObject.SetActive(false);
        }

        if (hp <= 0)
        {
            //GameOver
            PlayerCtrl.state = Moving.GameOver;
            SceneManager.LoadScene("GameOver");
        }
    }

    void UIUpdate()
    {
        m_Timer -= Time.deltaTime;

        if (m_Timer < 0)
        {
            m_Timer = 0.0f;
            Time.timeScale = 0.0f;      //�Ͻ����� ȿ��. start���� Ǯ����� ���� ������
            PlayerCtrl.state = Moving.GameOver;
            SceneManager.LoadScene("GameOver");
        }
           

        if (timerText != null)
        {
            if (m_Timer < 20.0f)
            {
                timerText.color = Color.red;
                timerText.fontSize = 55;
                timerText.fontStyle = FontStyle.Bold;
                
            }
            else if (m_Timer < 40.0f)
            { 
                timerText.color = Color.yellow;
                timerText.fontSize = 50;
            }
            timerText.text = m_Timer.ToString("F2");
        }
    }
    
    public void AddScore(int Value = 10)
    {
        score += Value;

        if (scoreText != null)
            scoreText.text = $"Score : {score}";
    }

    void AnimalRandGen()
    {
        for(int i = 0; i < 200; i++)
        {
            Vector3 RandomXYZ = new Vector3(
                Random.Range(-250.0f, 250.0f), 10.0f, Random.Range(-250.0f, 250.0f));
            //���� ���̿��ٰ� �߰������� 0~15���� �߰�
            RandomXYZ.y = m_RefMap.SampleHeight(RandomXYZ) + Random.Range(0.0f, 15.0f);

            int Kind = Random.Range(0, AnimalArr.Length);   //������ ����� �ִ� �迭 �� �ε��� ��������
            GameObject go = Instantiate(AnimalArr[Kind]);   //�� �ε����� ����� �ִ� ���� �ν��Ͻ�
            go.transform.SetParent(AnimalGroup);      //�θ� AnimalGroup���� �ض�
            go.transform.position = RandomXYZ;
            if (Kind == 2)
                go.transform.position = new Vector3(go.transform.position.x, 4, go.transform.position.z);
            go.transform.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
        }
    }
}
