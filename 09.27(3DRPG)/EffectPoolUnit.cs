using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class EffectPoolUnit : MonoBehaviour
{
    public float m_Delay = 1.0f;        //Ǯ�� ȯ���ǰ� ��� 1�� ���� �͵� ����� ����
    DateTime m_InactiveTime;        //Active���� ���� �ð� : ���� �� 1�� ������ üũ�ϱ� ���� ���

    EffectPool m_ObjectPool;
    string m_EffectName;

    //---ParticleAutoDestroy�� ���� �ʿ��� Ÿ��
    public enum DESTROY_TYPE
    {
        Destroy,
        Inactive,
    }

    DESTROY_TYPE m_DestroyType = DESTROY_TYPE.Inactive;    //Ǯ�� ȯ���ϴ°� ��Ģ�̶� �⺻�� Inactive;

    //�Ȳ����� Loop ���� ��ƼŬ�� �����ϱ� ���Ͽ� LifeTime ���� ����(���ӽð�) ���
    //�ش� �Ӽ����� ������ �� ������ ����, ������ isPlaying���� ����
    float m_LifeTime = 0.0f;
    float m_CurLifeTime;
    ParticleSystem[] m_Particles;

    // Start is called before the first frame update
    void Start()
    {
        m_Particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_LifeTime > 0.0f)
        {
            m_CurLifeTime += Time.deltaTime;
            if (m_LifeTime <= m_CurLifeTime)
            {
                DestroyParticles();
                m_CurLifeTime = 0.0f;
            }
        }
        else {
            bool isPlay = false;
            for (int i = 0; i < m_Particles.Length; i++)
            {
                if (m_Particles[i].isPlaying)   //��ƼŬ�� ��������� üũ
                {
                    isPlay = true;
                    break;
                }
            }

            if (!isPlay)
            {
                DestroyParticles();
            }
        }
    }

    void DestroyParticles()
    {
        switch (m_DestroyType)
        {
            case DESTROY_TYPE.Destroy:      //�޸�Ǯ�� ����X
                Destroy(gameObject);
                break;
            case DESTROY_TYPE.Inactive:     //�޸�Ǯ�� ����
                gameObject.SetActive(false); 
                break;
        }
    }

    public void SetObjectPool(string effectName, EffectPool objectPool)
    {
        m_EffectName = effectName;  //� ����Ʈ ����
        m_ObjectPool = objectPool;  //� Ǯ���� �����ϴ� ����Ʈ����
        ResetParent();
    }

    public void ResetParent()
    {
        transform.SetParent(m_ObjectPool.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public bool IsReady()
    {
        if (!gameObject.activeSelf) //false : Ǯ�� �� �ִ� ����
        {
            TimeSpan timeSpan = DateTime.Now - m_InactiveTime;
            //���� �ð� - ��Ƽ�긦 ���� ���� �ð��� timeSpan�� ����
            if (timeSpan.TotalSeconds > m_Delay)  //�ð��� ��ü, ��, ��, �ʷ� ������ ���� �� ����
            {
                //��Ƽ�갡 ������ 1���̻� ������ ���� ����
                return true;
            }
        }
        
        return false;
    }

    private void OnDisable()
    {
        //��Ȱ��ȭ �� ������ ȣ��Ǵ� �Լ�(��ũ��Ʈ�� ������Ʈ��) <----> OnEnable
        //��Ƽ�갡 ���� �� �޸� Ǯ�� �ٽ� ȯ��
        m_InactiveTime = DateTime.Now;
        m_ObjectPool.AddPoolUnit(m_EffectName, this);
        
    }

}
