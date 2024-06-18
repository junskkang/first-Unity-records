using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public GameObject bamsongiPrefab;

    public GameObject shieldSkill;
    public bool isShield = false;
    Vector3 shieldOffVec = new Vector3(0, 1.5f, -15.0f);
    Vector3 shieldOnVec = new Vector3(0, 1.5f, -5.0f);
    float onSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.inst.state == GameState.GameEnd) return;

        if (!isShield)
        {
            if (GameManager.inst.curMC > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject bamsongi = Instantiate(bamsongiPrefab);
                    //bamsongi.GetComponent<BamsongiController>().Shoot(new Vector3(0, 200, 2000));

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Vector3 worldDir = ray.direction;
                    bamsongi.GetComponent<BamsongiController>().Shoot(worldDir.normalized * 3500);

                    GameManager.inst.curMC -= 1;
                }
            }

        }


        if (Input.GetMouseButtonDown(1) && GameManager.inst.curMp > 20)
        {
            GameManager.inst.curMC = GameManager.inst.maxMC;

            GameManager.inst.curMp -= 20;
        }

        if (Input.GetKeyDown(KeyCode.Space) && GameManager.inst.curMp > 0.0f)
        {           

            isShield = true;
        }
        
        if (Input.GetKeyUp(KeyCode.Space) || GameManager.inst.curMp <= 0.0f) 
        {
            isShield = false;
        }

        if (isShield)
        {
            GameManager.inst.curMp -= (5.0f * Time.deltaTime);
            shieldSkill.transform.position = Vector3.MoveTowards(shieldOnVec, shieldOffVec, onSpeed);
        }
        else
        {
            shieldSkill.transform.position = Vector3.MoveTowards(shieldOffVec, shieldOnVec, onSpeed);
        }
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "EnemyBam")
        {
            GameManager.inst.TakeDamage(7.0f);

            Destroy(coll.gameObject, 1.0f);
        }
    }

}
