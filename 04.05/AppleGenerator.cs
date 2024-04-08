using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleGenerator : MonoBehaviour
{
    public GameObject applePrefabs;
    public GameObject arrowPrefabs;
    float span = 2.5f;            //�� �� ���� ������ ������
    float delta = 0;              //�ð� �帧 ���
    
    //int ratio = 3;
    //float m_DwSpeedCtrl = -0.1f;  //��ü ���� �ӵ��� �����ϱ� ���� ����

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
                span *= 0.97f;
                if (span <= 0.7f)
                    span = 0.7f;
                GameObject go = Instantiate(applePrefabs);   //������ �ν��Ͻ� �ҷ�����
                int px = Random.Range(-8, 9);                 //������ x ��ǥ�� ���� ����
                go.transform.position = new Vector3(px, 7, 0);//������ x ��ǥ������ ����
            }
        }

        ////���̵� ������
        //m_DwSpeedCtrl -= (Time.deltaTime * 0.005f); //���ϼӵ� ���� �������� �ϱ�
        //if (m_DwSpeedCtrl < -0.3f)
        //    m_DwSpeedCtrl = -0.3f;

        //span -= (Time.deltaTime * 0.03f);
        //if (span < 0.1f) //�����ֱ� ���� ª������
        //    span = 0.1f;

        ////������ ����Ǯ��
        //if (GameDirector.m_State == GameState.GameIng)
        //{
        //    this.delta += Time.deltaTime;     //�ð� ���ϱ�
        //    if (this.delta > this.span)       //2�ʸ��� ����
        //    {
        //        this.delta = 0;               //�ð� �ʱ�ȭ

        //        GameObject go = null;
        //        int dice = Random.Range(1, 11);
        //        if (dice <= this.ratio) // 30% Ȯ���� ���
        //        {
        //            go = Instantiate(applePrefabs);   //������ �ν��Ͻ� �ҷ�����
        //            go.GetComponent<AppleController>().m_DownSpeed = m_DwSpeedCtrl;
        //        }
        //        else                    // 70% Ȯ���� ȭ��
        //        {
        //            go = Instantiate(arrowPrefabs);   //������ �ν��Ͻ� �ҷ�����
        //            go.GetComponent<ArrowController>().m_DownSpeed = m_DwSpeedCtrl;
        //        }

        //        int px = Random.Range(-8, 9);                 //������ x ��ǥ�� ���� ����
        //        go.transform.position = new Vector3(px, 7, 0);//������ x ��ǥ������ ����
        //    }
        //}
    }
}
