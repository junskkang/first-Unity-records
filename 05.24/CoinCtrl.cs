using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCtrl : MonoBehaviour
{
    float flowSpeed = 5.0f;
    float chaseSpeed = 10.0f;
    Vector3 moveVec = Vector3.zero;
    Vector3 moveDir = Vector3.zero;
    [HideInInspector] public HeroCtrl refHero = null;
    void Start()
    {
        //refHero = GameObject.FindObjectOfType<HeroCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * flowSpeed * Time.deltaTime;

        ChaseHero();

        Destroy(gameObject, 5.0f);
    }

    void ChaseHero()
    {
        if (refHero == null) return;

        moveDir = refHero.transform.position - transform.position;
        if (moveDir.magnitude <= 5.0f)
        {
            moveVec = moveDir.normalized;
            transform.position += moveVec * chaseSpeed * Time.deltaTime;
        }
    }
}
