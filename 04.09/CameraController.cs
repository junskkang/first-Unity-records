using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        this.player = GameObject.Find("cat");
    }

    
    void Update()
    {
        //카메라의 이동이 캐릭터의 y축에 따라서만 이동되도록함
        Vector3 playerPos = this.player.transform.position;
        transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z); ;
    }
}
