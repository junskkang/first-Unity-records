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
        //ī�޶��� �̵��� ĳ������ y�࿡ ���󼭸� �̵��ǵ�����
        Vector3 playerPos = this.player.transform.position;
        transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z); ;
    }
}
