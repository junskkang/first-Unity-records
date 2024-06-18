using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //Shoot(new Vector3(0, 200, 2000));
        Destroy(this.gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);
    }

    void OnCollisionEnter(Collision coll)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play();
    }
}
