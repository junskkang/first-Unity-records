using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //������ Ÿ�� ������Ʈ�� Transform����
    public float dist = 10.0f;      //ī�޶���� ���� �Ÿ�
    public float height = 3.0f;     //ī�޶��� ���� ����
    public float dampTrace = 20.0f; //�δ��� ������ ���� ����

    Vector3 playerVec = Vector3.zero;

    Vector3 startPos = Vector3.zero;
    Vector3 dragPos = Vector3.zero;

    Vector3 toStartPos = Vector3.zero;
    Quaternion rotation;
    float rotateSpeed = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
    }

    //Update �Լ� ȣ�� ���� �ѹ��� ȣ��Ǵ� �Լ��� LateUpdate ���
    //������ Ÿ���� �̵��� ����� ���Ŀ� ī�޶� �����ϱ� ���� LateUpdate�� ���
    // Update is called once per frame
    void LateUpdate()
    {


        playerVec = targetTr.position;
        playerVec.y += 1.2f;

        //ī�޶��� ��ġ�� ��������� dist ������ŭ �������� ��ġ�ϰ�
        //height ������ŭ ���� �ø�
        transform.position = Vector3.Lerp(transform.position
                                        , targetTr.position 
                                        - (targetTr.forward * dist)
                                        + (Vector3.up * height)
                                        , Time.deltaTime * dampTrace);

        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.position = playerVec;

        if (Input.GetMouseButtonDown(1))
        {
            ClickMouse();
        }
        else if (Input.GetMouseButton(1))
        {
            DragMouse();
        }
    }

    public void ClickMouse()
    {
        startPos = Input.mousePosition;
    }

    public void DragMouse()
    {
        dragPos = Input.mousePosition;

        toStartPos = dragPos - startPos;

        //
        //
        //toStartPos.Normalize();

        if (startPos.y > dragPos.y) //���� ���� ȸ��
            this.transform.rotation = new Quaternion(this.transform.rotation.x + rotateSpeed * Time.deltaTime,
                this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
        if (startPos.y < dragPos.y) //���� �Ʒ��� ȸ��
            this.transform.rotation = new Quaternion(this.transform.rotation.x - rotateSpeed * Time.deltaTime,
                this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);

        //rotation = Quaternion.LookRotation(Vector3.right, toStartPos);
        //transform.rotation = rotation;
    }
}
