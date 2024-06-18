using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllyCtrl : MonoBehaviour
{
    public GameObject particle;
    public Material[] textures;
    public Image Hpbar;
    float curHp = 100;
    float maxHp = 100;

    Quaternion firstRot = Quaternion.identity;
    Quaternion lastRot = new Quaternion(0, -180, 0, 0);
    float rotTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        int idx = Random.Range(0, textures.Length);
        GetComponentInChildren<MeshRenderer>().material = textures[idx];

        Destroy(gameObject, 7.0f);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(RotateAI());
    }

    IEnumerator RotateAI()
    {
        yield return new WaitForSeconds(1.5f);

        rotTimer += Time.deltaTime;
        transform.rotation = Quaternion.Lerp(firstRot, lastRot, rotTimer / 10);

        particle.SetActive(true);

        if (GameManager.inst.curHp < GameManager.inst.maxHp)
        {
            GameManager.inst.curHp += 2.0f * Time.deltaTime;

            if (GameManager.inst.curHp >= GameManager.inst.maxHp)
                GameManager.inst.curHp = GameManager.inst.maxHp;
        }

        if (GameManager.inst.curMp < GameManager.inst.maxMp)
        {
            GameManager.inst.curMp += 1.0f * Time.deltaTime;

            if (GameManager.inst.curMp >= GameManager.inst.maxMp)
                GameManager.inst.curMp = GameManager.inst.maxMp;
        }
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
            GameManager.inst.AddScore(-20);
            //몬스터 제거
            Destroy(gameObject);
        }
    }
}
