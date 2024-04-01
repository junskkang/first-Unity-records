using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroCtrl : MonoBehaviour
{
    public GameObject Bullet_Prefab;  //유니티에서 연결

    // Start is called before the first frame update
    void Start()
    {
        Bullet_Prefab = Resources.Load("Bullet_Prefab") as GameObject;  //스크립트로 연결 구현
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) == true) 
        {
            Instantiate(Bullet_Prefab, transform.position, Quaternion.identity);
            //Bullet_Prefab의 복사본을 현재위치에서 회전시키지 않고 만들어줘
        }
    }
}
