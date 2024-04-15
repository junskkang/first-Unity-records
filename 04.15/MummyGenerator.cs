using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyGenerator : MonoBehaviour
{
    CameraController CameraController;
    public GameObject mummyPrefab;
    float spawn = 2.0f;
    float delta = 0f;
    // Start is called before the first frame update
    void Start()
    {
        CameraController = GameObject.FindObjectOfType<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraController.state == Moving.Stop)
        {
            delta += Time.deltaTime;
            if (delta > spawn)
            {
                delta = 0.0f;
                GameObject mummy = Instantiate(mummyPrefab);
                int ran = Random.Range(5, 11);
                mummy.transform.localScale = new Vector3(ran, ran, ran);
                Vector3 cameraPos = CameraController.transform.position;
                mummy.transform.position = new Vector3(cameraPos.x, 0, cameraPos.z + 30);
                
            }
        }
    }
}
