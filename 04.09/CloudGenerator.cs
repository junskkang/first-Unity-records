using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    public GameObject CloudWavePrefab;
    GameObject player;
    float createHeight = 10.0f;  //�ִ� ���� ���� ����
    float recentHeight = -2.5f;  //������ ������ �������� ����

    void Start()
    {
        this.player = GameObject.Find("cat"); 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = this.player.transform.position;
        //���� ���̿� ������ ����
        if (recentHeight < playerPos.y + createHeight)
        {
            SpawnCloudWave(recentHeight);
            recentHeight += 2.5f;
        }
    }

    void SpawnCloudWave(float a_Height)
    {
        int a_HideCount = 0;  //���̿� ���� �� ���� ���� ���ΰ�
        if (a_Height < 20.0f)
            a_HideCount = 0;
        else if (a_Height < 40.0f)
            a_HideCount = Random.Range(0, 2);  // 0~1�� ���߱�
        else if (a_Height < 60.0f)
            a_HideCount = Random.Range(0, 3);  // 0~2�� ���߱�
        else if (a_Height < 80.0f)
            a_HideCount = Random.Range(1, 3);  // 1~2�� ���߱�
        else if (a_Height < 100.0f)
            a_HideCount = Random.Range(2, 4);  // 2~3�� ���߱�
        else
            a_HideCount = Random.Range(3, 4);  // 3�� ���߱�
        
        GameObject go = Instantiate(CloudWavePrefab);
        go.transform.position = new Vector3(0.0f, a_Height, 0.0f);
        go.GetComponent<CloudWaveCtrl>().SetHideCloud(a_HideCount);
    }
}
