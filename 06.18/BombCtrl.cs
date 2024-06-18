using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCtrl : MonoBehaviour
{

    Quaternion firstRot = Quaternion.identity;
    Quaternion lastRot = new Quaternion(0, 180, 0, 0);
    float rotTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {       

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
    }
}
