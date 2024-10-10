using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    Dictionary<string, List<EffectPoolUnit>> m_DcEffectPool = 
        new Dictionary<string, List<EffectPoolUnit>>();

    int m_PresetSize = 3;   //�⺻������ 3���� ����� ����

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
            //TransparentFX : flares�� ������ ������ ��ü�� ���� ���̾�
            //Lens flare�� ��ֹ��� ���� �ʴ´�.
            //���� ��� ���� flare�� Default LayerMask�� ������ �������� �ڿ� �ִٸ�
            //flare�� ���������� ����

            for (int j = 0; j < m_PresetSize; j++)      //m_PresetSize = 3;
            {
                GameObject obj = Instantiate(a_Prefab) as GameObject;
                
                EffectPoolUnit objectPoolUnit = obj.GetComponent<EffectPoolUnit>();
                if (objectPoolUnit == null)
                {
                    objectPoolUnit = obj.AddComponent<EffectPoolUnit>();
                }

                obj.transform.SetParent(transform);     //Ǯ�� �ڽ����� ����
                obj.GetComponent<EffectPoolUnit>().SetObjectPool(effectName, this);
                if (obj.activeSelf)
                {
                    //���� �� ����Ʈ�� Ǯ�� �� �ִ� ���°� �ƴ�
                    //��Ƽ�긦 ���� OnDisable �̺�Ʈ �Լ��� ȣ��
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
                //�ε��� 0���� �غ� �Ǿ� ���� ������ �������� �翬�� �غ�Ǿ� ���� ���� ����
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
            //OnDisable()���� �޸�Ǯ�� �ٽ� ȯ�� 
        }

        obj.GetComponent<EffectPoolUnit>().SetObjectPool(effectName, this);
        StartCoroutine(MySetActiveCo(obj));
        return obj;
    }

    IEnumerator MySetActiveCo(GameObject obj)
    {
        yield return new WaitForEndOfFrame();
        //�������� ������ ó���� �Ŀ� �ڷ�ƾ�� �̾ �����
        //���� ȭ���� �׸��� �۾��� ���� �Ŀ� ó���ϰ� ���� ���� ���� �� �ش� �ɼ� ���
        obj.SetActive(true);
    }


}
