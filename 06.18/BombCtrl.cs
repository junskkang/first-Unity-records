using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombCtrl : MonoBehaviour
{
    public Image Hpbar;
    float curHp = 100;
    float maxHp = 100;

    public Image countBar;
    float countdown = 4.5f;

    Quaternion firstRot = Quaternion.identity;
    Quaternion lastRot = new Quaternion(0, -180, 0, 0);
    float rotTimer = 0.0f;

    //폭발 효과 연결 변수
    public GameObject expEffect;

    // Start is called before the first frame update
    void Start()
    {       

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(RotateAI());

        countdown -= Time.deltaTime;

        if (countBar != null)
            countBar.fillAmount = countdown / 3.0f;

        if (countdown < 0)
        {
            countdown = 0;

            Explosion();

            Destroy(gameObject);
        }
    }

    IEnumerator RotateAI()
    {
        yield return new WaitForSeconds(1.5f);

        rotTimer += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(firstRot, lastRot, rotTimer / 10);
    }

    void Explosion()
    {
        GameManager.inst.TakeDamage(30);

        //폭발 효과 파티클 생성
        GameObject explosion = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration + 2.0f);

    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "AllyBam")
        {
            TakeDamage(50.0f);

            Destroy(coll.gameObject, 0.7f);
        }
    }

    void TakeDamage(float damage)
    {
        if (curHp <= 0.0f) return;

        curHp -= damage;

        if (curHp <= 0.0f)
            curHp = 0.0f;

        if (Hpbar != null)
            Hpbar.fillAmount = curHp / maxHp;

        if (curHp <= 0.0f)
        {
            GameManager.inst.AddScore(30);
            //몬스터 제거
            Destroy(gameObject);
        }
    }
}
