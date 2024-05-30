using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    HeroCtrl refHero = null;

    Vector3 moveStep;
    float moveSpeed = 15.0f;
    float bulletDamage = 20.0f;

    Vector3 spawnPos = Vector3.zero;

    
    void Start()
    {
        Destroy(gameObject, 3.0f);  //3초후 제거
        refHero = GetComponent<HeroCtrl>();
        //spawnPos = transform.position;
        //moveStep = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //총알 이동
        
        //this.transform.Translate(moveStep, Space.World);

        transform.position += moveStep * moveSpeed * Time.deltaTime;

        //화면밖 총알 제거
        Vector3 Pos = Camera.main.WorldToViewportPoint(transform.position);
        if (Pos.x >= 1.0f || Pos.x <= 0.0f || Pos.y <= 0.0f || Pos.y >= 1.0f)
            Destroy(this.gameObject);

    }

    public void BulletFire(Vector3 ownVec, float Speed = 15.0f, float damage = 10.0f)
    {
        moveStep = Vector3.right;

        spawnPos = ownVec;// + new Vector3(2.0f, 0.0f, 0.0f);

        transform.position = spawnPos;

        moveSpeed = Speed;
        bulletDamage = damage;
    }

    public void BulletSpawn(Vector3 startPos, Vector3 dirVec, float moveSpeed = 15.0f, float att = 20.0f)
    {
        spawnPos = startPos;

        transform.position = spawnPos;

        moveStep = dirVec;

        this.moveSpeed = moveSpeed;
        bulletDamage = att;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.name.Contains("Skill2") == true)
            Destroy(this.gameObject);

        if (coll.name.Contains("Skill3") == true)
            Destroy(this.gameObject);
    }
}
