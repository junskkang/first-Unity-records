using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    //불렛프리팹을 주인공도 몬스터도 사용할 목적으로 한번만 로딩시키려고 함
    //같은 프리팹을 사용하는데 주인공과 몬스터에서 각각 로딩시키면 불필요한 낭비
    public static GameObject m_BulletPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        //게임매니저는 시작하면서 불렛프리팹을 로딩함
        m_BulletPrefab = Resources.Load("BulletPrefab") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
