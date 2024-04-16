using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyCtrl : MonoBehaviour
{
    Transform playerTr;
    [HideInInspector] public float m_MoveVelocity = 13.0f;  //초당 이동속도

    public int mummyHp = 2;
    // Start is called before the first frame update
    void Start()
    {
        //메인 카메라(플레이어)의 위치와 회전값을 알아오기
        //playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
        playerTr = Camera.main.transform;        //위에 것을 줄여서 이렇게 표현
    }

    // Update is called once per frame
    void Update()
    {
        //카메라 추적 이동 구현
        Vector3 a_MoveDir = Vector3.zero;
        a_MoveDir = playerTr.position - this.transform.position;  //몬스터로 부터 카메라를 향하는 거리
        a_MoveDir.y = 0.0f; //카메라가 언덕 위에 있거나 아래에 있더라도 수평으로만 회전하게끔

        //월드좌표 기준 움직임
        transform.forward = a_MoveDir;  //바라보는 방향을 카메라를 향하는 벡터로 하겠다.
        Vector3 a_StepVec = a_MoveDir.normalized * m_MoveVelocity * Time.deltaTime;  //방향벡터에 속도를 곱해서 이동시킴
        transform.Translate(a_StepVec, Space.World);  //좌표축은 월드좌표를 기준으로 이동하겠다.

        //로컬좌표 기준 움직임(Translate 함수의 디폴트 값)
        Vector3 a_StepVec2 = Vector3.forward * m_MoveVelocity * Time.deltaTime;
        transform.Translate(a_StepVec2, Space.Self);
       
        //싱글톤 패턴으로 저장된 게임매니저를 바로 불러옴
        //지형의 y축 값을 받아와 지형을 따라 이동할 수 있게 해줌
        float a_CacPosY = GameManager.Inst.m_RefMap.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, a_CacPosY, transform.position.z);

        if (a_MoveDir.magnitude < 5.0f)  //주인공과 부딪힌 상황
        {
            GameManager.Inst.DecreaseHp();  //싱글톤 패턴을 활용하여 카메라 hp감소시키기

            Destroy(gameObject);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("bamsongi") == true)
        {
            mummyHp -= 1;

            if (mummyHp <= 0)
            { 
                Destroy(gameObject);
                GameManager.Inst.score += 10;
            }
        }

    }
}
