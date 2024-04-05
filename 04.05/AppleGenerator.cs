using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleGenerator : MonoBehaviour
{
    public GameObject applePrefabs;
    float span = 2.5f;            //몇 초 마다 생성할 것인지
    float delta = 0;              //시간 흐름 기록

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
                int px = Random.Range(-8, 8);                 //임의의 x 좌표값 변수 생성
                go.transform.position = new Vector3(px, 7, 0);//임의의 x 좌표값에서 생성
            }
        }
    }
}
