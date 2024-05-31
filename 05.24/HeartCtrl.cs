using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartCtrl : MonoBehaviour
{
    Vector3 moveDir = Vector3.zero;
    float moveSpeed = 20.0f;
    HeroCtrl refHero = null;

    Vector3 moveSwitch = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        //15초 내에 먹지 않으면 삭제
        Destroy(gameObject, 15.0f);

        refHero = GameObject.FindObjectOfType<HeroCtrl>();
        moveDir = refHero.transform.position - this.transform.position;
        moveDir.x = moveDir.x + 5.0f;
        moveDir.y = moveDir.y + 5.0f;
        moveDir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (transform.position.x < CameraResolution.m_ScWMin.x + 0.1f)    //화면의 왼쪽끝
            moveDir.x = -moveDir.x;

        if (transform.position.x > CameraResolution.m_ScWMax.x - 0.1f)
            moveDir.x = -moveDir.x;

        if (transform.position.y < CameraResolution.m_ScWMin.y + 0.1f)
            moveDir.y = -moveDir.y;

        if (transform.position.y > CameraResolution.m_ScWMax.y - 0.1f)
            moveDir.y = -moveDir.y;

    }
}
