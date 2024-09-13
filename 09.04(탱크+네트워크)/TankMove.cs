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

    //��ũ�浹 ����ó��
    TankDamage tankDamage = null;
    Terrain terrain = null;
    Vector3 cacPos = Vector3.zero;

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

        //��ũ �浹 ���� ó���� ���� ����
        tankDamage = GetComponent<TankDamage>();
        terrain = FindObjectOfType<Terrain>();
    }

    // Update is called once per frame
    void Update()
    {
        //�ڽ��� ���� ��Ʈ��ũ ���ӿ�����Ʈ�� �ƴ� ��� Ű���� ���� �Ұ���
        if (pv.IsMine) //  && !GameManager.inst.chatInput.isFocused
        {
            if (GameManager.isChat) return;

            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            //ȸ���� �̵� ó��
            tr.Rotate(Vector3.up * rotSpeed * h * Time.deltaTime);
            tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);

            UnitCollUpdate();
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

    void UnitCollUpdate()
    {
        if (tankDamage == null) return;

        if (tankDamage.currHp <= 0.0f) return;

        if (terrain == null) return;

        //���� ������ �������� �ʰ� �ϱ�
        float curHeight = terrain.SampleHeight(transform.position);
        if (transform.position.y < curHeight - 1.0f)
        {
            transform.position = new Vector3(transform.position.x,
                                curHeight + 1.0f, transform.position.z);

            if (rbody != null)
            {
                rbody.isKinematic = false;
                rbody.velocity = new Vector3(0.0f, rbody.velocity.y, 0.0f);
                rbody.angularVelocity = Vector3.zero;
                rbody.isKinematic = true;
            }
        }

        //��ũ�� ���� ������ ����� ���ϰ� ����
        cacPos = transform.position;
        if (245.0f < transform.position.x)
            cacPos.x = 245.0f;
        if (245.0f < transform.position.z)
            cacPos.z = 245.0f;
        if (transform.position.x < -245.0f)
            cacPos.x = -245.0f;
        if (transform.position.z < -245.0f)
            cacPos.z = -245.0f;

        transform.position = cacPos;

        //��ũ�� ������ ���� ���ϵ��� �ٷ� �����
        if (transform.position.y < curHeight + 8.0f)
        if (Vector3.Dot(transform.up, Vector3.up) <= 0.0f)
        {
            //������ �����Ϳ� ��ũ�� �����͸� �����Ͽ� �� ������ 90���� �Ѿ���� ��
            //��ũ�� �����͸� �������ѹ���
            transform.up = Vector3.up;
            rbody.isKinematic = false;
            rbody.angularVelocity = new Vector3(0, 0, 0);
            rbody.isKinematic = true;
            }

    }
}
