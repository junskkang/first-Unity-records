using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    GameObject player;
    void Start()
    {
        player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < player.transform.position.y - 10.0f)
        {
            Destroy(gameObject);
        }
    }
}
