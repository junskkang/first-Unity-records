using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMsCtrl : MonoBehaviour
{
    float moveSpeed = 12.0f;     //�̵��ӵ�
    float rotSpeed = 100.0f;    //ȸ���ӵ�
    //�Ϻη� Ÿ���� ���� ���ư��� ������ ��Ʋ� ���������� ���󰡴� ��ó�� ���̰� �ϴ� ȿ��

    //����ź ����
    [HideInInspector] public GameObject targetObj = null;   //Ÿ������ ����
    Vector3 desireDir = Vector3.zero;   //Ÿ���� ���ϴ� ����


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObj == null)  //Ÿ�� ������Ʈ�� ������
            FindEnemy();        //Ÿ���� ã���ִ� �Լ�

        if (targetObj != null)  //Ÿ�� ������Ʈ�� ������
            BulletHoming();     //Ÿ���� ���� ���ư��� ���� �ൿ �Լ�
        else
            transform.Translate(transform.right * moveSpeed * Time.deltaTime, Space.World);
    }

    void FindEnemy()
    {
        //���� �±׸� �ް� �ִ� ���ӿ�����Ʈ ã�ƿ��� �Լ�
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Monster");

        if (enemyList.Length <= 0) return; //������ �ִ� ���Ͱ� �ϳ��� ������ ����

        GameObject findMonster = null;
        Vector3 cacVec = Vector3.zero;
        MonsterCtrl refMonster = null;
        for (int i = 0; i < enemyList.Length; i++)
        {
            refMonster = enemyList[i].GetComponent<MonsterCtrl>();  //

            if (refMonster == null) continue;   //���� ��Ʈ�� ������ ����

            //if (refMonster.homingLockOn != null) continue;
            if (refMonster.homingLockOn1 != null && refMonster.homingLockOn2 != null) continue;
            //���� �Ǿ� ������ ����

            findMonster = enemyList[i].gameObject;

            //if (refMonster.homingLockOn == null)
            //    refMonster.homingLockOn = this.gameObject;

            if (refMonster.homingLockOn1 == null) //�Ѹ����� �ִ� 2���� �̻��� Ÿ�� ����
                refMonster.homingLockOn1 = this.gameObject;
            else
                refMonster.homingLockOn2 = this.gameObject;

            break;
        }

        targetObj = findMonster;    //ȣ�ֹ̻����� Ÿ�ٿ��ٰ� ���� �� ���� ��ü�� ����

    }

    void BulletHoming()
    {
        //Ÿ�� ���� ȣ�ֹ̻����� ���ϴ� ���� ���ϱ�
        desireDir = targetObj.transform.position - transform.position;  
        desireDir.z = 0.0f;
        desireDir.Normalize();

        //Ÿ���� ���� ����ź ȸ����Ű��
        //Atan2 : �������κ��� ������ ���ϴ� �Լ� ù��° ������ �������� �ι�° �Ű����������� ����
        float angle = Mathf.Atan2(desireDir.y, desireDir.x) * Mathf.Rad2Deg;
        //XY�� ����̱� ������ Z�� ������ ����Ű�� Vector3.forward�� �������� ȸ��������� ��
        Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);    //������ ���ʹϿ�����

        //ù��° �Ű������� �����̼� ���� �ι�° �Ű����� ����ŭ ����° �Ű������� �ӵ��� ȸ����Ű�� �Լ�
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                     targetRot, rotSpeed * Time.deltaTime);

        //����ź�� �ٶ󺸴� ���������� �����̰� �ϱ�
        transform.Translate(transform.right * moveSpeed * Time.deltaTime, Space.World);

    }
}
