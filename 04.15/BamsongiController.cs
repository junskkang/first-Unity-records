using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    GameObject m_OverlapBlock;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //Shoot(new Vector3(0, 200, 2000)); 
        Destroy(this.gameObject, 10.0f);     //�ڿ� ���ڴ� �����ð�. ���� �� ���� �� ��
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 dir)
    {
        GetComponent<Rigidbody>().AddForce(dir);
    }

    private void OnCollisionEnter(Collision collision)
    {

        GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play();

        Destroy(this.gameObject, 4.0f); //����Ʈ�� ������ ���̵��� ���� �Ŀ� �����ǵ���

        if (collision.gameObject.tag == "Enemy")
        {
            //����� ���� �Ⱥ��̰� ����
            GetComponent<SphereCollider>().enabled = false;  //�浹 �Ұ����ϰ� �����

            MeshRenderer[] a_ChildList = 
                gameObject.GetComponentsInChildren<MeshRenderer>();  //���� ������ �� ��������
            for(int i = 0; i < a_ChildList.Length; i++)
            {
                a_ChildList[i].enabled = false;
            }

            //Destroy(collision.gameObject); //�浹�� �� ������ �ӹ���Ʈ�ѿ��� ����
        }

    }
}
