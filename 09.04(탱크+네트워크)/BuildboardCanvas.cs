using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildboardCanvas : MonoBehaviour
{
    Transform mainCameraTr;
    // Start is called before the first frame update
    void Start()
    {
        //스테이지에 있는 메인 카메라의 transform 컴포넌트 추출
        mainCameraTr = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = mainCameraTr.forward;
    }
}
