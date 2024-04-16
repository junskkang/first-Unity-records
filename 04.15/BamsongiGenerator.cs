using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject bamsongiPrefab;
    CameraController cameraController;
    void Start()
    {
        
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameObject bamsongi = Instantiate(bamsongiPrefab);
            //bamsongi.GetComponent<BamsongiController>().Shoot(new Vector3(0, 200, 2000));
            bamsongi.transform.position = Camera.main.transform.position
                +Camera.main.transform.forward*1.5f;   //약간 총구 앞에서 날아가는 느낌으로다가
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //마우스 클릭 위치에 레이저 광선을 만들어서
            Vector3 worldDir = ray.direction;                             //그 레이저 광선과의 직선거리를 구해서
            
            bamsongi.GetComponent<BamsongiController>().Shoot(worldDir.normalized * 3500);  //그 위치로 던져!
        }
    }
}
