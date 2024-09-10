using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    private Transform tr;
    //ray�� ���鿡 �´� ��ġ�� ������ ����
    private RaycastHit hit;

    //�ͷ��� ȸ�� �ӵ�
    public float rotSpeed = 10.0f;

    //PhotonView ������Ʈ ����
    private PhotonView pv = null;
    //���� ��Ʈ��ũ ��ũ�� �ͷ� ȸ������ ������ ����
    private Quaternion currRot = Quaternion.identity;

    

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        tr = GetComponent<Transform>();

        //�ʱ� ȸ���� ����
        currRot = tr.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //�ڽ��� ��ũ�� ���� ����
        if (pv.IsMine)
        {        
            //���� ī�޶󿡼� ���콺 Ŀ���� ��ġ�� ĳ���õǴ� Ray�� ����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //������ Ray�� Scene�信 ��� �������� ǥ��
            Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.green);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TERRAIN")))
            {
                //Ray�� ���� ��ġ�� ������ǥ�� ��ȯ
                Vector3 relative = tr.InverseTransformPoint(hit.point);
                //��ź��Ʈ �Լ��� Atan2�� �� �� ���� ������ ���
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                //rotSpeed ������ ������ �ӵ��� Y�� ȸ��
                tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
            }
            else
            {
                //origin : ������ �������� 
                //direction : ������ ����
                Vector3 orgVec = ray.origin + ray.direction * 2000.0f;
                //�ݴ�������� ��� ���� : ���� �ٱ����� �׸��� �־ �ȿ��� ��� ��Ʈ�� ���� ����
                ray = new Ray(orgVec, -ray.direction);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TurretPickObject")))
                {
                    Vector3 cacVec = hit.point - transform.position;
                    Quaternion rotate = Quaternion.LookRotation(cacVec.normalized);
                    rotate.eulerAngles = new Vector3(transform.eulerAngles.x,
                        rotate.eulerAngles.y, transform.eulerAngles.z);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotSpeed);
                    transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, 0.0f);

                    ////Ray�� ���� ��ġ�� ������ǥ�� ��ȯ
                    //Vector3 relative = tr.InverseTransformPoint(hit.point);
                    ////��ź��Ʈ �Լ��� Atan2�� �� �� ���� ������ ���
                    //float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                    ////rotSpeed ������ ������ �ӵ��� Y�� ȸ��
                    //tr.Rotate(0, angle * Time.deltaTime * rotSpeed, 0);
                }
            }
        }
        else     //���� ��Ʈ��ũ �÷��̾��� ��ũ�� ���
        {
            //���� ȸ���������� ���Ź��� �ǽð� ȸ�������� �ε巴�� ȸ��
            tr.localRotation = Quaternion.Slerp(tr.localRotation, currRot, Time.deltaTime * rotSpeed);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   //�۽�
        {
            stream.SendNext(tr.localRotation);
        }
        else   //����
        {
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
