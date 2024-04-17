using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemGenerator : MonoBehaviour
{
    public GameObject applePrefab;
    public GameObject bombPrefab;
    float spawn = 1.0f;        //���� �ֱ�
    float delta = 0.0f;        //���� ī��Ʈ
    int ratio = 2;             //������ ���� Ȯ��

    float speed = -0.03f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.spawn)
        {
            this.delta = 0.0f;
            GameObject item;
            int dice = Random.Range(1, 11);
            if (dice <= this.ratio)
            {
                item = Instantiate(bombPrefab);
            }
            else
            {
                item = Instantiate(applePrefab);
            }
            float x = Random.Range(-1, 2);
            float z = Random.Range(-1, 2);
            item.transform.position = new Vector3(x, 4, z);
            item.GetComponent<ItemCtrl>().dropSpeed = this.speed;
        }
    }

    public void SetParameter(float spawn, float speed, int ratio)
    {
        this.spawn = spawn;
        this.speed = speed;
        this.ratio = ratio;
    }
}
