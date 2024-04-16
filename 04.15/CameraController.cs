using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum Moving
//{
//    IsMoving,
//    Stop,
//    GameOver
     
//}

public class CameraController : MonoBehaviour
{
    float m_WalkSpeed = 30.0f;
    float m_RotateSpeed = 0.5f;
    Vector3 m_StartPos;
    public Moving state;
    // Start is called before the first frame update
    void Start()
    {
        state = Moving.Stop;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) 
            state = Moving.IsMoving;
        else
            state = Moving.Stop;

        //이동 구현
        if (state != Moving.GameOver)
        {

            if (Input.GetKey(KeyCode.W))
                this.transform.position = new Vector3(this.transform.position.x,
                    this.transform.position.y, this.transform.position.z + m_WalkSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.S))
                this.transform.position = new Vector3(this.transform.position.x,
                    this.transform.position.y, this.transform.position.z - m_WalkSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.A))
                this.transform.position = new Vector3(this.transform.position.x - m_WalkSpeed * Time.deltaTime,
                    this.transform.position.y, this.transform.position.z);
            if (Input.GetKey(KeyCode.D))
                this.transform.position = new Vector3((this.transform.position.x + m_WalkSpeed * Time.deltaTime),
                    this.transform.position.y, this.transform.position.z);

            float a_CacPosY = GameManager.Inst.m_RefMap.SampleHeight(transform.position);
            transform.position = new Vector3(transform.position.x, 5 + a_CacPosY, transform.position.z);


            //카메라 시점 회전
            if (Input.GetMouseButtonDown(1))
                m_StartPos = Input.mousePosition;
            if (Input.GetMouseButton(1))
            {
                Vector3 endPos = Input.mousePosition;
                if (m_StartPos.x < endPos.x) //시점 오른쪽 회전
                    this.transform.rotation = new Quaternion(this.transform.rotation.x,
                        this.transform.rotation.y + m_RotateSpeed * Time.deltaTime, this.transform.rotation.z, this.transform.rotation.w);
                if (m_StartPos.x > endPos.x) //시점 왼쪽 회전
                    this.transform.rotation = new Quaternion(this.transform.rotation.x,
                        this.transform.rotation.y - m_RotateSpeed * Time.deltaTime, this.transform.rotation.z, this.transform.rotation.w);
                if (m_StartPos.y > endPos.y) //시점 위쪽 회전
                    this.transform.rotation = new Quaternion(this.transform.rotation.x + m_RotateSpeed * Time.deltaTime,
                        this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
                if (m_StartPos.y < endPos.y) //시점 아래쪽 회전
                    this.transform.rotation = new Quaternion(this.transform.rotation.x - m_RotateSpeed * Time.deltaTime,
                        this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
            }
        }
    }

}
