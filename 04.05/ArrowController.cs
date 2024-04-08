using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    GameObject player;
    GameObject GameD = null;

    public float m_DownSpeed = -0.1f;
    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("player");

        GameD = GameObject.Find("GameDirector");  //GameDirector라는 오브젝트를 불러옴
    }

    // Update is called once per frame
    void Update()
    {
        //프레임마다 등속으로 낙하시킨다
        transform.Translate(0, m_DownSpeed, 0);

        //화면 밖으로 나오면 오브젝트를 소멸시킨다.
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);          //부모오브젝트를 파괴
        }

        //충돌 판정
        Vector2 p1 = transform.position;             //화살의 중심좌표
        Vector2 p2 = this.player.transform.position; //플레이어의 중심좌표
        Vector2 dir = p1 - p2;                       //플레이어에서 화살로 향하는 벡터 값
        float d = dir.magnitude;                     //직선거리   
        float r1 = 0.5f;                             //화살의 반경
        float r2 = 0.95f;                             //플레이어의 반경

        if( d < r1+r2)  //직선거리가 두 오브젝트의 반경 합보다 작은경우
        {
            //GameDirector스크립트 안의 DecreaseHp함수 호출
            GameD.GetComponent<GameDirector>().DecreaseHp();

            Destroy (gameObject); //충돌이므로 파괴
        }

        if (GameDirector.m_State == GameState.GameEnd)
        {
            Destroy (gameObject);
        }
    }
}
