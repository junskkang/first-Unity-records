using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground1 : MonoBehaviour
{
    [SerializeField] private Transform target;      //현재 배경과 이어질 배경대상
    [SerializeField] private float scrollAmount;    //두 배경의 거리
    [SerializeField] private float moveSpeed;       //이동속도
    [SerializeField] private Vector3 moveDirection; //이동방향
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //현재 배경 moveDirection의 방향으로 moveSpeed를 가지고 이동
        this.transform.position += moveDirection * moveSpeed * Time.deltaTime;

        //캐릭터는 오른쪽으로 진행하는 느낌을 주기 위해 배경은 왼쪽(-x축)방향으로 이동시킬 것
        //그래서 두 배경사이의 거리 값에 -를 붙여주는 것!
        if (transform.position.x <= -scrollAmount)
        {
            transform.position = target.position - moveDirection * scrollAmount;
            //ex) 배경의 위치는 = 40 - (-1 * 40) = 다음 위치는 80이 될 것
        }
    }
}
