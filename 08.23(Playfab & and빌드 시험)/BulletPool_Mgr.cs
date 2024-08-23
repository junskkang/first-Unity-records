using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool_Mgr : MonoBehaviour
{
    [Header("--- Bullet Pool ---")]
    public GameObject BulletPrefab;
    //총알을 미리 생성해 저장할 리스트 자료형
    [HideInInspector] public List<BulletCtrl> m_BulletPool = new List<BulletCtrl>();

    //--- 싱글턴 패턴
    public static BulletPool_Mgr Inst = null;

    void Awake()
    {
        Inst = this;    
    }
    //--- 싱글턴 패턴

    // Start is called before the first frame update
    void Start()
    {
        //--- Bullet Pool
        //총알을 생성해 오브젝트 풀에 저장
        for(int i = 0; i < 50; i++)
        {
            //총알 프리팹을 생성
            GameObject a_Bullet = Instantiate(BulletPrefab);
            //생성한 총알을 Bullet_Mgr 밑으로 차일드화 하기
            a_Bullet.transform.SetParent(this.transform);
            //생성한 총알을 비활성화
            a_Bullet.SetActive(false);
            //생성한 총알을 오브젝트 풀에 추가
            BulletCtrl a_BL_Ctrl = a_Bullet.GetComponent<BulletCtrl>();
            a_BL_Ctrl.m_IsPool = true;
            m_BulletPool.Add(a_BL_Ctrl);
        }
        //--- Bullet Pool
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public BulletCtrl GetBulletPool()
    {
        //오브젝트 풀의 처음부터 끝까지 순회
 
        foreach(BulletCtrl a_BNode in m_BulletPool)
        {
            //비활성화 여부로 사용 가능한 Bullet을 판단
            if(a_BNode.gameObject.activeSelf == false)
            {
                a_BNode.gameObject.SetActive(true);
                return a_BNode;
            }
        }//foreach(BulletCtrl a_BNode in m_BulletPool)

        //대기하고 있는 총알이 하나도 없으면 이쪽으로 넘어고게 된다.
        //그럴 경우 총알을 새로 하나 더 추가로 만들어 준다.

        //총알 프리팹을 생성
        GameObject a_Bullet = Instantiate(BulletPrefab);
        //생성한 총알을 Bullet_Mgr 밑으로 차일드화 하기
        a_Bullet.transform.SetParent(this.transform);
        //생성한 총알을 비활성화
        //a_Bullet.SetActive(false);
        //생성한 총알을 오브젝트 풀에 추가
        BulletCtrl a_BL_Ctrl = a_Bullet.GetComponent<BulletCtrl>();
        a_BL_Ctrl.m_IsPool = true;
        m_BulletPool.Add(a_BL_Ctrl);

        //생성한 총알을 활성화
        a_Bullet.SetActive(true);
        return a_BL_Ctrl;

    }//public BulletCtrl GetALBulletPool()
}
