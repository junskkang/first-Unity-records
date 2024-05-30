using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill3Ctrl : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Zombi)
            Destroy(collision.gameObject);
        if (collision.gameObject.GetComponent<MonsterCtrl>().type == MonsterType.Missile)
            Destroy(collision.gameObject);


        //Debug.Log(collision.gameObject.name);
        //if (collision.gameObject.name.Contains("Enemy") == true)
        //    Destroy(collision.gameObject);
    }
}
