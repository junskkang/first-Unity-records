using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool_Mgr : MonoBehaviour
{
    [Header("Bullet Pool")]
    public GameObject BulletPrefab;
    //총알을 미리 생성해 저장할 리스트
    [HideInInspector] public List<BulletCtrl> m_BulletPool = new List<BulletCtrl>();

    //싱글턴 패턴
    public static BulletPool_Mgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        for (int i = 0; i < 50; i++)
        {
            //총알 프리팹 생성
            GameObject a_Bullet = Instantiate(BulletPrefab);
            //생성한 총알들 BulletPool_Mgr의 차일드화 하기
            a_Bullet.transform.SetParent(this.transform);
            //생성한 총알 비활성화 해두기
            a_Bullet.SetActive(false);
            //생성한 총알 오브젝트 풀 리스트에 추가
            BulletCtrl a_BL_Ctrl = a_Bullet.GetComponent<BulletCtrl>();
            a_BL_Ctrl.m_IsPool = true;
            m_BulletPool.Add(a_BL_Ctrl);
        }
    }

    public BulletCtrl GetBulletPool()
    {
        //오브젝트 풀의 처음부터 끝까지 순회
        foreach (BulletCtrl a_BNode in m_BulletPool)
        {
            //비활성화 여부로 사용가능한 Bullet을 판단
            if (a_BNode.gameObject.activeSelf == false)
            {
                a_BNode.gameObject.SetActive(true);
                return a_BNode;
            }
        }

        //대기하고 있는 총알이 하나도 없을 경우 새로운 총알을 생성해서 추가해줌
        //총알 프리팹 생성
        GameObject a_Bullet = Instantiate(BulletPrefab);
        //생성한 총알들 BulletPool_Mgr의 차일드화 하기
        a_Bullet.transform.SetParent(this.transform);
        //생성한 총알 비활성화 해두기
        a_Bullet.SetActive(false);
        //생성한 총알 오브젝트 풀 리스트에 추가
        BulletCtrl a_BL_Ctrl = a_Bullet.GetComponent<BulletCtrl>();
        a_BL_Ctrl.m_IsPool = true;
        m_BulletPool.Add(a_BL_Ctrl);

        a_Bullet.SetActive(true);
        return a_BL_Ctrl;
    }
}
