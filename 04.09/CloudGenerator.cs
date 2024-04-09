using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    public GameObject CloudFloorPrefab;
    float m_posY;
    PlayerController m_Player;
    GameObject CloudFloor1;
    void Start()
    {
        m_Player = GameObject.FindObjectOfType<PlayerController>();
        m_posY = m_Player.transform.position.y;

        CloudFloor1 = GameObject.Find("CloudFloor");
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((m_Player.transform.position.y)%2.5f == 0)
        {
            
            GameObject go = Instantiate(CloudFloorPrefab);
            float cloudY = CloudFloor1.transform.position.y + 5;

            go.transform.position = new Vector3(transform.position.x, cloudY, 0);
        }
    }
}
