using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool_Mgr : MonoBehaviour
{
    [Header("Bullet Pool")]
    public GameObject BulletPrefab;
    //�Ѿ��� �̸� ������ ������ ����Ʈ
    [HideInInspector] public List<BulletCtrl> m_BulletPool = new List<BulletCtrl>();

    //�̱��� ����
    public static BulletPool_Mgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            //�Ѿ� ������ ����
            GameObject a_Bullet = Instantiate(BulletPrefab);
            //������ �Ѿ˵� BulletPool_Mgr�� ���ϵ�ȭ �ϱ�
            a_Bullet.transform.SetParent(this.transform);
            //������ �Ѿ� ��Ȱ��ȭ �صα�
            a_Bullet.SetActive(false);
            //������ �Ѿ� ������Ʈ Ǯ ����Ʈ�� �߰�
            BulletCtrl a_BL_Ctrl = a_Bullet.GetComponent<BulletCtrl>();
            a_BL_Ctrl.m_IsPool = true;
            m_BulletPool.Add(a_BL_Ctrl);
        }
    }

    public BulletCtrl GetBulletPool()
    {
        //������Ʈ Ǯ�� ó������ ������ ��ȸ
        foreach (BulletCtrl a_BNode in m_BulletPool)
        {
            //��Ȱ��ȭ ���η� ��밡���� Bullet�� �Ǵ�
            if (a_BNode.gameObject.activeSelf == false)
            {
                a_BNode.gameObject.SetActive(true);
                return a_BNode;
            }
        }

        //����ϰ� �ִ� �Ѿ��� �ϳ��� ���� ��� ���ο� �Ѿ��� �����ؼ� �߰�����
        //�Ѿ� ������ ����
        GameObject a_Bullet = Instantiate(BulletPrefab);
        //������ �Ѿ˵� BulletPool_Mgr�� ���ϵ�ȭ �ϱ�
        a_Bullet.transform.SetParent(this.transform);
        //������ �Ѿ� ��Ȱ��ȭ �صα�
        a_Bullet.SetActive(false);
        //������ �Ѿ� ������Ʈ Ǯ ����Ʈ�� �߰�
        BulletCtrl a_BL_Ctrl = a_Bullet.GetComponent<BulletCtrl>();
        a_BL_Ctrl.m_IsPool = true;
        m_BulletPool.Add(a_BL_Ctrl);

        a_Bullet.SetActive(true);
        return a_BL_Ctrl;
    }
}
