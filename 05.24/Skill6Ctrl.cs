using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Skill6Ctrl : MonoBehaviour
{
    HeroCtrl refHero = null;

    //float radius = 1.0f;
    float velocity = 40.0f;
    //float angle = 0.0f;

    float attackSpeed = 1.2f;
    float attackTick = 0.0f;


    //선생님 풀이
    float angle = 0.0f; //각도 계산용
    float radius = 2.0f; //회전 반경
    float speed = 100.0f; //회전ㅅ ㅗㄱ도
    Vector3 parentPos = Vector3.zero;   //중심이 될 오브젝트 좌표
    float lifeTime = 0.0f;  //지속시간

    void Start()
    {        
        refHero = GameObject.FindObjectOfType<HeroCtrl>();

        Destroy(gameObject, refHero.skill6Duration);

        attackTick = 1.2f;
        //if (creature == null)
        //{
        //    creature = gameObject.GetComponentsInChildren<GameObject>(true);
        //}
        //

        //선생님 풀이
        //lifeTime = 12.0f;
        //refHero = transform.parent.parent.GetComponent<HeroCtrl>();   //부모의 부모를 찾아옴
        //transform.root.GetComponent<HeroCtrl>();            //root = 부모 중에서 찾아옴

    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(refHero.transform.position, Vector3.forward, velocity * Time.deltaTime);

        attackTick += Time.deltaTime;

        if (attackTick >= attackSpeed)
        {
            BulletFire();

            attackTick = 0.0f;
        }

        ////선생님풀이
        //lifeTime -= Time.deltaTime;
        //if (lifeTime <= 0.0f)
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //if (refHero == null || transform.parent == null)    //플레이어와 스킬루트가 널이면 리턴
        //{
        //    Destroy(gameObject);
        //    return;
        //}
        //angle += Time.deltaTime * speed;
        //if (360.0f < angle) // 0~360도를 순환하기 위한 코드
        //    angle -= 360.0f;

        //parentPos = transform.position;
        //transform.position = parentPos +
        //    new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
        //                Mathf.Sin(angle * Mathf.Deg2Rad) * radius, 0.0f);
    }

    void BulletFire()
    {
        if (refHero.isSkill5On)
        {
            GameObject doubleShoot = null;
            Vector3 pos = Vector3.zero;
            for (float yy = 0.6f; yy >= -1.0f; yy -= 0.8f)
            {
                if (-0.2f < yy && yy < 0.0f) continue;

                pos.y = yy;
                pos.x = 0.5f;

                doubleShoot = Instantiate(refHero.bulletPrefab);
                doubleShoot.transform.SetParent(refHero.bulletPool);
                BulletCtrl bulletCtrl = doubleShoot.GetComponent<BulletCtrl>();
                bulletCtrl.BulletFire(transform.position + pos);
            }
        }
        else
        {
            GameObject obj = Instantiate(refHero.bulletPrefab);
            obj.transform.SetParent(refHero.bulletPool);
            BulletCtrl bulletCtrl = obj.GetComponent<BulletCtrl>();
            bulletCtrl.BulletFire(transform.position);
        }

    }

    public void SubHeroSpawn(float angle, float lifeTime)
    {
        this.angle = angle;
        this.lifeTime = lifeTime;
    }




}
