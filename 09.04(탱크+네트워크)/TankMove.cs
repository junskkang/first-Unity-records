using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class TankMove : MonoBehaviourPunCallbacks, IPunObservable
{
    //��ũ�� �̵� �� ȸ�� �ӵ��� ��Ÿ���� ����
    public float moveSpeed = 20.0f;
    public float rotSpeed = 50.0f;

    //������ ������Ʈ�� �Ҵ��� ����
    private Rigidbody rbody;
    private Transform tr;

    //Ű���� �Է°� ����
    private float h, v;

    //PhotonView ������Ʈ�� �Ҵ��� ����
    private PhotonView pv = null;
    //����ī�޶� ������ CamPivot ���ӿ�����Ʈ
    public Transform camPivot;

    //��ġ ������ �ۼ����� �� ����� ���� ���� �� �ʱ갪 ����
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;

    void Start()
    {
        //������Ʈ �Ҵ�
        rbody = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        pv = GetComponent<PhotonView>();

        //Rigidbody�� �����߽��� ���� ����
        rbody.centerOfMass = new Vector3(0.0f, -2.5f, 0.0f);

        //PhotonView�� �ڽ��� ��ũ�� ���
        if (pv.IsMine)
        {
            //���� ī�޶� �߰��� SmoothFollw ��ũ��Ʈ�� ��������� ����
            Camera.main.GetComponent<SmoothFollow>().target = camPivot;
        }
        else
        {
            //���� ��Ʈ��ũ �÷��̾��� ��ũ�� �������� �̿����� ����
            rbody.isKinematic = true;
        }

        //���� ��ũ�� ��ġ �� ȸ������ ó���� ������ �ʱⰪ ����
        currPos = transform.position;
        currRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //�ڽ��� ���� ��Ʈ��ũ ���ӿ�����Ʈ�� �ƴ� ��� Ű���� ���� �Ұ���
        if (pv.IsMine)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            //ȸ���� �̵� ó��
            tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        }
        else //���� �÷��̾ ���̴� ���� �ƹ�Ÿ�� ����...
        {
            if (10.0f < (transform.position - currPos).magnitude)
            {
                //�߰� ���� ��ǥ�� ������ǥ�� �Ÿ����� 10m �̻��̸� ��� �������� ����
                transform.position = currPos;
            }
            else
            {
                //���� �÷��̾��� ��ũ�� ���Ź��� ��ġ���� �ε巴�� �̵���Ŵ
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10.0f);
            }
            tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10.0f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //���� �÷��̾��� ��ġ ���� �۽�
        if (stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else //���� �÷��̾� PC�� ���̴� IsMine�� �ƹ�Ÿ ������Ʈ ��ġ ���� ����
        {
            currPos = (Vector3) stream.ReceiveNext();
            currRot = (Quaternion) stream.ReceiveNext();
        }

    }
}
