using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretCtrl : MonoBehaviour
{
    private Transform tr;
    //ray�� ���鿡 �´� ��ġ�� ������ ����
    private RaycastHit hit;

    //�ͷ��� ȸ�� �ӵ�
    public float rotSpeed = 5.0f;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
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
    }
}
