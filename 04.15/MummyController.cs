using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyController : MonoBehaviour
{
    int m_Hp = 2;
    public float m_Speed = 30.0f;
    CameraController CameraController;
    Rigidbody m_rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        CameraController = GetComponent<CameraController>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 mummyPos = transform.position;
        mummyPos.y = 0;
        Vector3 cameraPos = CameraController.transform.position; 
        cameraPos.y = 0;
        Vector3 dirVec = cameraPos - mummyPos;  //이동할 방향 벡터
        
        Vector3 mummyCome = dirVec.normalized * m_Speed * Time.deltaTime;

        this.transform.position = mummyCome;
        //m_rigidbody.MovePosition(m_rigidbody.position + mummyCome);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("bamsongi") == true)
        {
            m_Hp -= 1;

            if (m_Hp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
