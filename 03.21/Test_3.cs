using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���Ǻ� ������ (��ó����)
// ��ó���� : #�� �پ��ִ� ��ɾ��� ����(������)������ �����ϴ� ��ɾ�
public class Test_3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if KOREA
        Debug.Log("�̰��� �ѱ� �����Դϴ�.");
#elif ENGLISH
        Debug.Log("�̰��� ���� �����Դϴ�.");
#elif CHINA
        Debug.Log("�̰��� �߱��� �����Դϴ�.");
#endif

    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
#if UNITY_STANDALONE_WIN
        Debug.Log("�����쿡�� ����");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "�����쿡�� ����" + "</size>");
#elif UNITY_ANDROID
        Debug.Log("�ȵ���̵忡�� ����");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "�ȵ���̵忡�� ����" + "</size>");
#elif UNITY_IOS
        Debug.Log("���������� ����");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "���������� ����" + "</size>");
#elif UNITY_WEBGL
        Debug.Log("������ ����");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "������ ����" + "</size>");
#endif

    }
}
