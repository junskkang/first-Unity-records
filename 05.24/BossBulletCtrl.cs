using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletCtrl : MonoBehaviour
{
    HeroCtrl refHero = null;
    Vector3 targetPos = Vector3.zero;
    Vector3 dirVec = Vector3.zero;
    Vector3 moveStep = Vector3.zero;
    Quaternion cacRot = Quaternion.identity;
    float moveSpeed = 10.0f;
    float damage = 20.0f;
    
    
    void Start()
    {
        refHero = GameObject.FindObjectOfType<HeroCtrl>();

        //총알 이동
        targetPos = refHero.transform.position;
        dirVec = targetPos - transform.position;
        dirVec.z = 0.0f;
        dirVec.Normalize();

        transform.forward = dirVec;


    }

    // Update is called once per frame
    void Update()
    {

        moveStep = dirVec * moveSpeed * Time.deltaTime;
        this.transform.Translate(moveStep, Space.World);

        //화면 밖 총알 제거
        Vector3 Pos = Camera.main.WorldToViewportPoint(transform.position);
        if (Pos.x >= 1.0f || Pos.x <= 0.0f || Pos.y <= 0.0f || Pos.y >= 1.0f)
            Destroy(this.gameObject);
    }
}
