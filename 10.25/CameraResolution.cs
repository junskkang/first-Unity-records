using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    Camera a_Cam = null;
    public GameObject UI_MaskGroup;

    //Viewport 좌표를 월드좌표로 Viewport to World
    public static Vector3 minVtW = new Vector3(-9.0f, 5.0f, 0.0f);
    public static Vector3 maxVtW = new Vector3(9.0f, 5.0f, 0.0f);

    Vector3 defaultCamPos = Vector3.zero;
    public GameObject zoomTarget = null;
    float cal = 0.0f;
    float zoomSpeed = 2.0f;
    public static bool isZoom = false;
    // Start is called before the first frame update
    void Start()
    {
        a_Cam = GetComponent<Camera>();
        defaultCamPos = a_Cam.transform.position;
        Rect rect = a_Cam.rect;
        float scaleHeight = ((float)Screen.width / Screen.height) /
                                    ((float) 16 / 9);
        float scaleWidth = 1.0f / scaleHeight;

        if(scaleHeight < 1.0f)
        {
            rect.height = scaleHeight;
            rect.y = (1.0f - scaleHeight) / 2.0f;
        }
        else
        {
            rect.width = scaleWidth;
            rect.x = (1.0f - scaleWidth) / 2.0f;
        }

        a_Cam.rect = rect;

        //OnPreCull(); //Mask 역할

        if (UI_MaskGroup != null)
            UI_MaskGroup.SetActive(true);

        //Viewport의 월드좌표 구해놓기
        //화면의 좌측하단 코너의 월드좌표 
        Vector3 a_VpMin = Vector3.zero;
        minVtW = Camera.main.ViewportToWorldPoint(a_VpMin);

        //화면의 우측상단 코너의 월드좌표 
        Vector3 a_VpMax = Vector3.one;
        maxVtW = Camera.main.ViewportToWorldPoint(a_VpMax);
    }

    //void OnPreCull() => GL.Clear(true, true, Color.black);

    //private void OnPreCull()
    //{
    //    //Rect rect = GetComponent<Camera>().rect;
    //    //Rect newRect = new Rect(0, 0, 1, 1);
    //    //GetComponent<Camera>().rect = newRect;
    //    GL.Clear(true, true, Color.black);
    //    //GetComponent<Camera>().rect = rect;
    //}

    // Update is called once per frame
    void Update()
    { 
        ZoomIn(zoomTarget);
    }

    public void ZoomIn(GameObject target = null)
    {
        if (target == null)
        {
            transform.position = defaultCamPos;
            a_Cam.orthographicSize = 7;
            isZoom = false;
        }
        else
        {
            isZoom = true;
            Vector3 outZ = new Vector3(target.transform.position.x, target.transform.position.y, -10);            
                        
            if ((outZ - transform.position).magnitude > 0.01f) 
                transform.position = Vector3.Lerp(transform.position, outZ, Time.deltaTime * zoomSpeed * 2);
            else 
                transform.position = outZ;
            
            if (a_Cam.orthographicSize >= 5.01f)
            {
                cal += Time.deltaTime * zoomSpeed;
                a_Cam.orthographicSize = Mathf.Lerp(7, 5, cal);
            }
            else
            {
                cal = 0.0f;
                a_Cam.orthographicSize = 5;
            }              
        }
    }
}
