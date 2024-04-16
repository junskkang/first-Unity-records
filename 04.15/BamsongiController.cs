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
        Destroy(this.gameObject, 10.0f);     //뒤에 숫자는 지연시간. 몇초 후 삭제 해 줘
        
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

        Destroy(this.gameObject, 4.0f); //이펙트가 끝까지 보이도록 몇초 후에 삭제되도록

        if (collision.gameObject.tag == "Enemy")
        {
            //밤송이 외형 안보이게 정리
            GetComponent<SphereCollider>().enabled = false;  //충돌 불가능하게 만들기

            MeshRenderer[] a_ChildList = 
                gameObject.GetComponentsInChildren<MeshRenderer>();  //외형 렌더러 다 꺼버리기
            for(int i = 0; i < a_ChildList.Length; i++)
            {
                a_ChildList[i].enabled = false;
            }

            //Destroy(collision.gameObject); //충돌된 적 삭제는 머미컨트롤에서 구현
        }

    }
}
