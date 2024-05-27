using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    public GameObject UIMaskGruop = null;

    //��ũ���� ������ǥ ScW : Screen To World
    public static Vector3 m_ScWMin = new Vector3(-10.0f, -5.0f, 0.0f);  //�����ϴ� ������ǥ
    public static Vector3 m_ScWMax = new Vector3(10.0f, 5.0f, 0.0f);    //������� ������ǥ

    void Awake()
    {
        if(UIMaskGruop != null)
            UIMaskGruop.SetActive(true);

    }
    // Start is called before the first frame update
    void Start()
    {
        Camera a_Cam = GetComponent<Camera>();
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

        //OnPreCull(); //Mask ����

        //��ũ������ ���� ��ǥ ���ϱ�
        Vector3 a_ScMin = Vector3.zero;
        m_ScWMin = a_Cam.ViewportToWorldPoint(a_ScMin);

        Vector3 a_ScMax = Vector3.one;
        m_ScWMax = a_Cam.ViewportToWorldPoint(a_ScMax);

        
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
       
    }
}
