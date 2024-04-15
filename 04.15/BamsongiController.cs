using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    GameObject m_OverlapBlock;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //Shoot(new Vector3(0, 200, 2000)); 
        Destroy(this.gameObject, 10.0f);     //뒤에 숫자는 지연시간. 몇초 후 삭제 해 줘
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);
    }

    private void OnCollisionEnter(Collision collision)
    {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<ParticleSystem>().Play();

    }
}
