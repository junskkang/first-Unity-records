using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCtrl : MonoBehaviour
{
    public enum MoveType
    {
        WAY_POINT,
        LOOK_AT,
        DAYDREAM
    }

    //�̵����
    public MoveType moveType = MoveType.WAY_POINT;

    //�̵��ӵ�
    public float speed = 1.0f;
    //ȸ�� �� ȸ�� �ӵ��� ������ ���
    public float damping = 3.0f;

    private Transform tr;
    private Transform camTr;
    private CharacterController cc;
    //��������Ʈ�� ������ �迭
    private Transform[] points;
    //������ �̵��ؾ� �� ��ġ �ε��� ����
    private int nextIdx = 1;

    public static bool isStopped = false;


    void Start()
    {        
        tr = GetComponent<Transform>();
        camTr = Camera.main.GetComponent<Transform>();
        cc = GetComponent<CharacterController>();


        //WayPointGroup ���ӿ�����Ʈ ���ϵ�� �ִ� ��� Point�� Transform������Ʈ�� ����
        points = GameObject.Find("WaypointGroup").GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStopped) return;

        switch (moveType)
        {
            case MoveType.WAY_POINT:
                MoveWayPoint();
                break;

            case MoveType.LOOK_AT:
                MoveLookAt();
                break;
            case MoveType.DAYDREAM:
                break;
        }
    }

    void MoveWayPoint()
    {
        //���� ��ġ���� ���� ��������Ʈ�� �ٶ󺸴� ���͸� ���
        Vector3 direction = points[nextIdx].position - tr.position;
        //����� ������ ȸ�� ������ ���ʹϾ� Ÿ������ ����
        Quaternion rot = Quaternion.LookRotation(direction);
        //���� �������� ȸ���ؾ� �� �������� �ε巴�� ȸ�� ó��
        tr.rotation = Quaternion.Slerp(tr.rotation, rot, Time.deltaTime * damping);

        //���� �������� �̵� ó��
        tr.Translate(Vector3.forward * Time.deltaTime * speed);
    }

    void MoveLookAt()
    {
        //���� ī�޶� �ٶ󺸴� ����
        Vector3 dir = camTr.TransformDirection(Vector3.forward);    //���带 > ���� ��ǥ��
        Vector3 dir2 = camTr.forward;
        //dir�������� �ʴ� speed��ŭ�� �̵�
        cc.SimpleMove(dir * speed);
        //rigidbody�� �̵� ��Ű�� : AddForce() , velocity
        //Transform�� �̵� ��Ű�� : Translate(), position
        //CharacterController�� �̵� ��Ű�� : SimpleMove()
    }

    private void OnTriggerEnter(Collider other)
    {
        //��������Ʈ�� �浹 ���� �Ǵ�
        if (other.CompareTag("WAY_POINT"))
        {
            //�� ������ ��������Ʈ�� �������� �� ó�� �ε����� ����
            nextIdx = (++nextIdx >= points.Length) ? 1 : nextIdx;
        }
    }
}
