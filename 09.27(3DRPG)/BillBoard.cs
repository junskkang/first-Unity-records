using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    Transform cameraTr = null;
    // Start is called before the first frame update
    void Start()
    {
        cameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.forward = Camera.main.transform.forward;
    }
}
