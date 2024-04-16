using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //지형의 높이값을 알아오기 위한 변수
    public Terrain m_RefMap = null;

    //UI관련 변수
    int hp = 3;
    public Image[] LifeImg;
    public Text timerText;
    float timer = 60.0f;
    public Text scoreText;
    public int score = 0;

    //미이라 관련 변수
    PlayerCtrl playerCtrl;
    public GameObject MummyRoot;
    float span = 1.0f;    //미이라 스폰주기
    float delta = 0.0f;   //미이라 스폰 주기 계산용 변수
    float m_MvSpeedCtrl = 13.0f;  //전체 미이라 이동 속도를 제어하기 위한 변수

    //GameScene 안에서만 적용되는 싱글톤 패턴
    public static GameManager Inst = null;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        playerCtrl = GameObject.FindObjectOfType<PlayerCtrl>();
    }

    
    void Update()
    {
        if(PlayerCtrl.state == Moving.Stop)
            MummyGenerator();

        UIUpdate();
    }

    void MummyGenerator()  //캐릭터가 멈췄을 때 눈 앞에 랜덤하게 미이라를 생성하는 함수
    {
        //난이도 조절
        //미이라 이속 증가
        m_MvSpeedCtrl += (Time.deltaTime * 0.1f);
        if (35.0f < m_MvSpeedCtrl)
            m_MvSpeedCtrl = 35.0f;
        //스폰 주기 감소
        span -= (Time.deltaTime * 0.03f);
        if (span < 0.2f)
            span = 0.2f;

        this.delta += Time.deltaTime;
        if (span < delta)
        {
            delta = 0.0f;

            //z좌표 축으로 50미터 떨어진 곳 좌표 저장
            Vector3 CamForw = Camera.main.transform.forward;
            CamForw.y = 0.0f;
            CamForw.Normalize();                   //CamForw의 값 자체를 Normalize한 값으로 바꿔버림
            //Vector3 a_Norm = CamForw.normalized;   //CamForw의 값 자체는 변하지 않아. 계산한 값을 다른 곳에 넣어줌
            //방향벡터에 50~51m 더 떨어진 곳으로 던져놓음
            CamForw = CamForw * (float)Random.Range(50.0f, 52.0f);

            //x좌표 축으로 -36~ 35까지의 랜덤 좌표 저장
            Vector3 CacX = Camera.main.transform.right;
            CacX.y = 0.0f;
            CacX.Normalize();
            CacX = CacX * Random.Range(-36.0f, 36.0f);

            //x좌표, z좌표의 변화값을 적용시킨 인스턴스 생성
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
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer = 0.0f;
            PlayerCtrl.state = Moving.GameOver;
            SceneManager.LoadScene("GameOver");
        }
           

        if (timerText != null)
        {
            if (timer < 20.0f)
            {
                timerText.color = Color.red;
                timerText.fontSize = 55;
                timerText.fontStyle = FontStyle.Bold;
                
            }
            else if (timer < 40.0f)
            { 
                timerText.color = Color.yellow;
                timerText.fontSize = 50;
            }
            timerText.text = timer.ToString("F2");
        }
            
        

        if (scoreText != null)
            scoreText.text = $"Score : {score}"; 
    }
}
