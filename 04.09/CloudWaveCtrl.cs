using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloudWaveCtrl : MonoBehaviour
{
    GameObject player;
    float destroyDistance = 10.0f;   //���ΰ� �Ʒ��� 10m

    public GameObject[] Clouds;      //������ ���� ���ӿ�����Ʈ �迭 ����
    void Start()
    {
        player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;

        //������ ���ΰ����κ��� 10m �Ʒ� �Ÿ��̸� �ı�
        if (transform.position.y < playerPos.y - destroyDistance)
        {
            Destroy(gameObject);
        }
    }

    public void SetHideCloud(int a_Count)
    {//a_Count �� ���� ������ ���� ������ 
        List<int> active = new List<int>();     //���� ����  
        for (int i =0; i < Clouds.Length; i++)  //���ڿ� ����������ŭ ���� �߰�
        {
            active.Add(i);
        }

        for(int i = 0; i < a_Count; i++)             //������ ������ŭ ���� ����
        {
            int ran = Random.Range(0, active.Count); //���� ������ �ִ����� �Ͽ� ���� ��÷
            Clouds[active[ran]].SetActive(false);    //���� ��ȣ�� ���� ���ڿ� �ش��ϴ� ���� ����

            active.RemoveAt(ran);                    //���� ���� ����
        }

        active.Clear();                              //���� ����
    }
}
