using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    HeroCtrl refHero = null;

    Vector3 moveStep = Vector3.zero;
    float moveSpeed = 15.0f;
    float bulletDamage = 10.0f;
    
    Vector3 spawnPos = Vector3.zero;
    void Start()
    {
        refHero = GetComponent<HeroCtrl>();
        spawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //총알 이동
        moveStep = Vector3.right * moveSpeed * Time.deltaTime;
        this.transform.Translate(moveStep, Space.World);

        //화면밖 총알 제거
        Vector3 rightMaxPos = Camera.main.WorldToViewportPoint(transform.position);
        if (rightMaxPos.x >= 1.0f)
            Destroy(this.gameObject);
    }

    public void BulletFire(Vector3 ownVec, float Speed = 15.0f, float damage = 10.0f)
    {
        spawnPos = ownVec + new Vector3(2.0f, 0.0f, 0.0f);

        transform.position = spawnPos;

        moveSpeed = Speed;
        bulletDamage = damage;


    }
}
