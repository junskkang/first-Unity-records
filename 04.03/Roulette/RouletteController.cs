using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteController : MonoBehaviour
{
    float rotSpeed = 0;    //회전속도

    void Start()
    {
        Application.targetFrameRate = 60;   //프레임을 60으로 고정
        QualitySettings.vSyncCount = 0;  
        //모니터 주사율(프레임율)이 다른 컴퓨터일 경우 캐릭터 조작시 빠르게 움직일 수 있다.
    }

    // Update is called once per frame
    void Update()
    {
        //클릭하면 회전속도를 설정
        if (Input.GetMouseButtonDown(0)) //왼쪽 마우스버튼을 클릭하면
        {
            this.rotSpeed = 10;
        }


        //회전속도만큼 룰렛 회전  1프레임에 10도씩
        transform.Rotate(0,0, this.rotSpeed);

        //Rotate()의 구조
        //Vector3 a_Rot = transform.transform.eulerAngles;  // eulerAngels = 0~360의 값
        //a_Rot.z += this.rotSpeed;
        //transform.transform.eulerAngles = a_Rot;

        //transform.eulerAngels.z : 0~359.999f의 값으로 환산하여 보여줌. 마이너스던 540도던 

        //룰렛 속도 감소
        this.rotSpeed *= 0.98f;
    }
}
