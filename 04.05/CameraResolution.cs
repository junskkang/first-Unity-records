using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Camera a_Cam = GetComponent<Camera>();
        Rect rect = a_Cam.rect;
        float scaleHeight = ((float)Screen.width / Screen.height)/ ((float) 16/ 9);
        float scaleWidth = 1.0f / scaleHeight;

        if (scaleHeight < 1.0f)
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

        OnPreCull();  
    }

    void OnPreCull() => GL.Clear(true, true, Color.black); //해상도 변화에 따른 마스킹 테이프 용도
    // Update is called once per frame
    void Update()
    {
        
    }
}
