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
            bamsongi.transform.position = Camera.main.transform.position;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //���콺 Ŭ�� ��ġ�� ������ ������ ����
            Vector3 worldDir = ray.direction;                             //�� ������ �������� �����Ÿ��� ���ؼ�
            
            bamsongi.GetComponent<BamsongiController>().Shoot(worldDir.normalized * 3500);  //�� ��ġ�� ����!
        }
    }
}
