using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildboardCanvas : MonoBehaviour
{
    Transform mainCameraTr;
    // Start is called before the first frame update
    void Start()
    {
        //���������� �ִ� ���� ī�޶��� transform ������Ʈ ����
        mainCameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = mainCameraTr.forward;
    }
}
