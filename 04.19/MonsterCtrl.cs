using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCtrl : MonoBehaviour
{
    float m_MaxHp = 100.0f;
    float m_CurHp = 100.0f;
    public Image HpBarUI = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name.Contains("BulletPrefab") == true)
        {
            TakeDamage(10.0f);

            Destroy(coll.gameObject);   //부딪힌 총알 제거
        }

    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)    //디스트로이가 한프레임 늦게 되기 때문에 혹시나
            return;             //보상이 두번 들어오게 되는 경우 방지

        GameMgr.Inst.DamageText((int)a_Value, this.transform.position); //데미지 폰트 함수 호출

        m_CurHp -= a_Value;

        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if(HpBarUI != null)
            HpBarUI.fillAmount = m_CurHp/m_MaxHp;   //몬스터 체력바UI 표시

        if (m_CurHp <= 0.0f) //몬스터 사망처리
        {
            //보상

            Destroy(gameObject);    //몬스터 제거
        }
    }
}
