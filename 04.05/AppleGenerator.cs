using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleGenerator : MonoBehaviour
{
    public GameObject applePrefabs;
    public GameObject arrowPrefabs;
    float span = 2.5f;            //몇 초 마다 생성할 것인지
    float delta = 0;              //시간 흐름 기록
    
    //int ratio = 3;
    //float m_DwSpeedCtrl = -0.1f;  //전체 낙하 속도를 제어하기 위한 변수

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.m_State == GameState.GameIng)
        {
            this.delta += Time.deltaTime;     //시간 더하기
            if (this.delta > this.span)       //2초마다 생성
            {
                this.delta = 0;               //시간 초기화
                span *= 0.97f;
                if (span <= 0.7f)
                    span = 0.7f;
                GameObject go = Instantiate(applePrefabs);   //프리팹 인스턴스 불러오기
                int px = Random.Range(-8, 9);                 //임의의 x 좌표값 변수 생성
                go.transform.position = new Vector3(px, 7, 0);//임의의 x 좌표값에서 생성
            }
        }

        ////난이도 스케일
        //m_DwSpeedCtrl -= (Time.deltaTime * 0.005f); //낙하속도 점점 빨라지게 하기
        //if (m_DwSpeedCtrl < -0.3f)
        //    m_DwSpeedCtrl = -0.3f;

        //span -= (Time.deltaTime * 0.03f);
        //if (span < 0.1f) //스폰주기 점점 짧아지게
        //    span = 0.1f;

        ////선생님 문제풀이
        //if (GameDirector.m_State == GameState.GameIng)
        //{
        //    this.delta += Time.deltaTime;     //시간 더하기
        //    if (this.delta > this.span)       //2초마다 생성
        //    {
        //        this.delta = 0;               //시간 초기화

        //        GameObject go = null;
        //        int dice = Random.Range(1, 11);
        //        if (dice <= this.ratio) // 30% 확률로 사과
        //        {
        //            go = Instantiate(applePrefabs);   //프리팹 인스턴스 불러오기
        //            go.GetComponent<AppleController>().m_DownSpeed = m_DwSpeedCtrl;
        //        }
        //        else                    // 70% 확률로 화살
        //        {
        //            go = Instantiate(arrowPrefabs);   //프리팹 인스턴스 불러오기
        //            go.GetComponent<ArrowController>().m_DownSpeed = m_DwSpeedCtrl;
        //        }

        //        int px = Random.Range(-8, 9);                 //임의의 x 좌표값 변수 생성
        //        go.transform.position = new Vector3(px, 7, 0);//임의의 x 좌표값에서 생성
        //    }
        //}
    }
}
