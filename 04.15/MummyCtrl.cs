using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyCtrl : MonoBehaviour
{
    Transform playerTr;
    [HideInInspector] public float m_MoveVelocity = 13.0f;  //�ʴ� �̵��ӵ�

    public int mummyHp = 2;
    // Start is called before the first frame update
    void Start()
    {
        //���� ī�޶�(�÷��̾�)�� ��ġ�� ȸ������ �˾ƿ���
        //playerTr = GameObject.Find("Main Camera").GetComponent<Transform>();
        playerTr = Camera.main.transform;        //���� ���� �ٿ��� �̷��� ǥ��
    }

    // Update is called once per frame
    void Update()
    {
        //ī�޶� ���� �̵� ����
        Vector3 a_MoveDir = Vector3.zero;
        a_MoveDir = playerTr.position - this.transform.position;  //���ͷ� ���� ī�޶� ���ϴ� �Ÿ�
        a_MoveDir.y = 0.0f; //ī�޶� ��� ���� �ְų� �Ʒ��� �ִ��� �������θ� ȸ���ϰԲ�

        //������ǥ ���� ������
        transform.forward = a_MoveDir;  //�ٶ󺸴� ������ ī�޶� ���ϴ� ���ͷ� �ϰڴ�.
        Vector3 a_StepVec = a_MoveDir.normalized * m_MoveVelocity * Time.deltaTime;  //���⺤�Ϳ� �ӵ��� ���ؼ� �̵���Ŵ
        transform.Translate(a_StepVec, Space.World);  //��ǥ���� ������ǥ�� �������� �̵��ϰڴ�.

        //������ǥ ���� ������(Translate �Լ��� ����Ʈ ��)
        Vector3 a_StepVec2 = Vector3.forward * m_MoveVelocity * Time.deltaTime;
        transform.Translate(a_StepVec2, Space.Self);
       
        //�̱��� �������� ����� ���ӸŴ����� �ٷ� �ҷ���
        //������ y�� ���� �޾ƿ� ������ ���� �̵��� �� �ְ� ����
        float a_CacPosY = GameManager.Inst.m_RefMap.SampleHeight(transform.position);
        transform.position = new Vector3(transform.position.x, a_CacPosY, transform.position.z);

        if (a_MoveDir.magnitude < 5.0f)  //���ΰ��� �ε��� ��Ȳ
        {
            GameManager.Inst.DecreaseHp();  //�̱��� ������ Ȱ���Ͽ� ī�޶� hp���ҽ�Ű��

            Destroy(gameObject);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("bamsongi") == true)
        {
            mummyHp -= 1;

            if (mummyHp <= 0)
            { 
                Destroy(gameObject);
                GameManager.Inst.score += 10;
            }
        }

    }
}
