using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefabs;
    float span = 1.8f;            //�� �� ���� ������ ������
    float delta = 0;              //�ð� �帧 ���

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameDirector.m_State == GameState.GameIng)
        {
            this.delta += Time.deltaTime;     //�ð� ���ϱ�
            if (this.delta > this.span)       //2�ʸ��� ����
            {
                this.delta = 0;               //�ð� �ʱ�ȭ
                span *= 0.93f;
                if (span <= 0.18f)
                    span = 0.18f;
                GameObject go = Instantiate(arrowPrefabs);   //������ �ν��Ͻ� �ҷ�����
                int px = Random.Range(-8, 8);                 //������ x ��ǥ�� ���� ����
                go.transform.position = new Vector3(px, 7, 0);//������ x ��ǥ������ ����
            }
        }
    }
}
