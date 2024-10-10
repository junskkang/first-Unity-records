using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class EffectPoolUnit : MonoBehaviour
{
    public float m_Delay = 1.0f;        //풀에 환원되고 적어도 1초 지난 것들 사용할 예정
    DateTime m_InactiveTime;        //Active껐을 때의 시간 : 꺼진 후 1초 지남을 체크하기 위해 사용

    EffectPool m_ObjectPool;
    string m_EffectName;

    //---ParticleAutoDestroy를 위해 필요한 타입
    public enum DESTROY_TYPE
    {
        Destroy,
        Inactive,
    }

    DESTROY_TYPE m_DestroyType = DESTROY_TYPE.Inactive;    //풀에 환원하는게 원칙이라 기본값 Inactive;

    //안꺼지고 Loop 도는 파티클을 제어하기 위하여 LifeTime 설정 버프(지속시간) 사용
    //해당 속성값이 있으면 이 변수로 제어, 없으면 isPlaying으로 제어
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
                if (m_Particles[i].isPlaying)   //파티클이 재생중인지 체크
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
            case DESTROY_TYPE.Destroy:      //메모리풀로 관리X
                Destroy(gameObject);
                break;
            case DESTROY_TYPE.Inactive:     //메모리풀로 관리
                gameObject.SetActive(false); 
                break;
        }
    }

    public void SetObjectPool(string effectName, EffectPool objectPool)
    {
        m_EffectName = effectName;  //어떤 이펙트 인지
        m_ObjectPool = objectPool;  //어떤 풀에서 관리하는 이펙트인지
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
        if (!gameObject.activeSelf) //false : 풀에 들어가 있는 상태
        {
            TimeSpan timeSpan = DateTime.Now - m_InactiveTime;
            //현재 시간 - 액티브를 껐을 때의 시간을 timeSpan에 저장
            if (timeSpan.TotalSeconds > m_Delay)  //시간을 전체, 시, 분, 초로 나눠서 받을 수 있음
            {
                //액티브가 꺼진지 1초이상 지나면 재사용 예정
                return true;
            }
        }
        
        return false;
    }

    private void OnDisable()
    {
        //비활성화 될 때마다 호출되는 함수(스크립트든 오브젝트든) <----> OnEnable
        //액티브가 꺼질 때 메모리 풀에 다시 환원
        m_InactiveTime = DateTime.Now;
        m_ObjectPool.AddPoolUnit(m_EffectName, this);
        
    }

}
