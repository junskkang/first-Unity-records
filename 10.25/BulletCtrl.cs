using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    Vector3 dirVec = Vector3.right;
    float moveSpeed = 70.0f;        //날아가는 속도

    [HideInInspector] public float curAttDamage = 0;
    [HideInInspector] public GameObject hunterAttackEff = null;
    [HideInInspector] public HunterUnit hunterUnit = null;
    [HideInInspector] public GameObject target = null;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
        //Invoke("EffectOn", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (NotifyCtrl.isNotify) return;

        transform.position += dirVec * moveSpeed * Time.deltaTime;

        //화면 밖을 벗어난 총알 바로 삭제
        if (CameraResolution.maxVtW.x + 1.0f < transform.position.x ||
            CameraResolution.minVtW.x - 1.0f > transform.position.x ||
            CameraResolution.maxVtW.y + 1.0f < transform.position.y ||
            CameraResolution.minVtW.y - 1.0f > transform.position.y)
            Destroy(gameObject);
    }

    public void BulletSpawn(Vector3 startPos, Vector3 dirVec, float AttDamage, GameObject effect, HunterUnit whosAttack)
    {
        this.dirVec = dirVec;
        this.curAttDamage = AttDamage;
        this.hunterAttackEff = effect;
        this.hunterUnit = whosAttack;

        transform.position = new Vector3 (startPos.x, startPos.y, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Monster")
        {     
            //데미지는 직접 주는 걸로
            //coll.GetComponent<Monster_Ctrl>().TakeDamage(curAttDamage, hunterUnit.gameObject);

            //GameObject effect;
            ////이펙트 생성
            //if (hunterAttackEff != null)
            //{
            //    effect = Instantiate(hunterAttackEff) as GameObject;
            //    effect.transform.position = coll.transform.position;
            //    Destroy(effect, 0.25f);
            //}

            Destroy(this.gameObject);
        }
    }

    public IEnumerator EffectOn(Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        GameObject effect;
        //이펙트 생성
        if (hunterAttackEff != null)        {           

            effect = Instantiate(hunterAttackEff) as GameObject;
            
            effect.transform.position = position;
            Destroy(effect, 0.25f);
        }
    }
}
