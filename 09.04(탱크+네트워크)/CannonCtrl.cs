using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonCtrl : MonoBehaviour
{
    private Transform tr;
    public float rotSpeed = 1000.0f;

    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * rotSpeed;
        //tr.Rotate(angle, 0, 0);          

        //���� ī�޶󿡼� ���콺 Ŀ���� ��ġ�� ĳ���õǴ� ����ĳ��Ʈ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("TERRAIN"))) 
        {
            //���ſ��� ��Ʈ�� ���ϴ� ����
            Vector3 cacVec = hit.point - transform.position;
            //�ش� ������ ȸ������ ����
            Quaternion rotate = Quaternion.LookRotation(cacVec.normalized); 
            //x�����θ� ȸ���� �ٰ��̱⿡ �������� ������Ű�� 
            rotate.eulerAngles = new Vector3(rotate.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
            //�����Լ��� ���� �������ϰ� ȸ���ǵ�����
            transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
            //y,z�����δ� ȸ���� ������ �ٽ� �� �� �ʱⰪ���� �������ѹ���
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, 0.0f);
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
                rotate.eulerAngles = new Vector3(rotate.eulerAngles.x,
                    transform.eulerAngles.y, transform.eulerAngles.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * 10.0f);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0.0f, 0.0f);
            }
        }

        //���� ���� ����
        Vector3 angle = transform.localEulerAngles;
        if (angle.x < 180.0f)
        {
            if (5.0f < angle.x)
                angle.x = 5.0f;
        }
        else
        {
            if (angle.x < 330.0f)
                angle.x = 330.0f;
        }
        transform.localEulerAngles = angle;

    }
}
