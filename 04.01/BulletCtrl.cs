using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    float m_Speed = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1.0f);   //스크립트의 부모인 게임오브젝트 파괴
    }

    // Update is called once per frame
    void Update()
    {
        //속도 = 거리/시간
        //속도 * 시간 = 거리
        transform.position += Vector3.right * m_Speed * Time.deltaTime; // 한프레임동안 이동하게 될 거리
    }
}
