using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    Dictionary<string, List<EffectPoolUnit>> m_DcEffectPool = 
        new Dictionary<string, List<EffectPoolUnit>>();

    int m_PresetSize = 3;   //기본적으로 3개씩 만들어 놓음

    public static EffectPool Inst;
    private void Awake()
    {
        Inst = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCreate("FX_Hit_01");
        StartCreate("FX_Attack01_01");
        StartCreate("FX_AttackCritical_01");
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    public void StartCreate(string effectName)
    {
        List<EffectPoolUnit> listObjectPool = null;
        if (m_DcEffectPool.ContainsKey(effectName))
        {
            listObjectPool = m_DcEffectPool[effectName];
        }
        else
        {
            m_DcEffectPool.Add(effectName, new List<EffectPoolUnit>());
            listObjectPool = m_DcEffectPool[effectName];
        }

        GameObject a_Prefab = Resources.Load<GameObject>("Effect/" + effectName);
        if ((a_Prefab != null))
        {
            var results = a_Prefab.GetComponentsInChildren<Transform>();
            for (int k = 0; k < results.Length; k++)
                results[k].gameObject.layer = LayerMask.NameToLayer("TransparentFX");
            //TransparentFX : flares와 투명값을 가지는 물체를 위한 레이어
            //Lens flare의 장애물이 되지 않는다.
            //예를 들어 만약 flare가 Default LayerMask의 투명한 유리조각 뒤에 있다면
            //flare와 간섭현상이 생김

            for (int j = 0; j < m_PresetSize; j++)      //m_PresetSize = 3;
            {
                GameObject obj = Instantiate(a_Prefab) as GameObject;
                
                EffectPoolUnit objectPoolUnit = obj.GetComponent<EffectPoolUnit>();
                if (objectPoolUnit == null)
                {
                    objectPoolUnit = obj.AddComponent<EffectPoolUnit>();
                }

                obj.transform.SetParent(transform);     //풀의 자식으로 넣음
                obj.GetComponent<EffectPoolUnit>().SetObjectPool(effectName, this);
                if (obj.activeSelf)
                {
                    //아직 이 이펙트는 풀에 들어가 있는 상태가 아님
                    //액티브를 끄면 OnDisable 이벤트 함수가 호출
                    obj.SetActive(false);
                }
                else
                {
                    AddPoolUnit(effectName, obj.GetComponent<EffectPoolUnit>());
                }
            }
        }
    }

    public void AddPoolUnit(string effectName, EffectPoolUnit unit)
    {
        List<EffectPoolUnit> listObjectPool = m_DcEffectPool[effectName];
        if (listObjectPool != null)
        {
            listObjectPool.Add(unit);
        }
    }
    public GameObject GetEffectObj(string effectName, Vector3 position, Quaternion rotation) 
    {
        List<EffectPoolUnit> listObjectPool = null;
        if (m_DcEffectPool.ContainsKey(effectName))
        {
            listObjectPool = m_DcEffectPool[effectName];
        }
        else
        {
            m_DcEffectPool.Add(effectName, new List<EffectPoolUnit>());
            listObjectPool = m_DcEffectPool[effectName];
        }

        if (listObjectPool == null) return null;     
        
        if (0 < listObjectPool.Count)
        {
            if (listObjectPool[0] != null && listObjectPool[0].IsReady())
            {
                //인덱스 0번도 준비가 되어 있지 않으면 나머지도 당연히 준비되어 있지 않은 상태
                EffectPoolUnit unit = listObjectPool[0];
                listObjectPool.Remove(listObjectPool[0]);
                unit.transform.position = position;
                unit.transform.rotation = rotation;
                StartCoroutine(MySetActiveCo(unit.gameObject));
                return unit.gameObject;
            }
        }

        GameObject a_Prefab = Resources.Load<GameObject>("Effect/" + effectName);
        GameObject obj = Instantiate(a_Prefab) as GameObject;

        EffectPoolUnit objectPoolUnit = obj.GetComponent<EffectPoolUnit>();
        if (objectPoolUnit == null)
        {
            objectPoolUnit = obj.AddComponent<EffectPoolUnit>();
            //OnDisable()에서 메모리풀로 다시 환원 
        }

        obj.GetComponent<EffectPoolUnit>().SetObjectPool(effectName, this);
        StartCoroutine(MySetActiveCo(obj));
        return obj;
    }

    IEnumerator MySetActiveCo(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        //프레임이 완전히 처리된 후에 코루틴이 이어서 실행됨
        //보통 화면을 그리는 작업이 끝난 후에 처리하고 싶은 일이 있을 때 해당 옵션 사용
        obj.SetActive(true);
    }


}
