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

            Destroy(coll.gameObject);   //�ε��� �Ѿ� ����
        }

    }

    public void TakeDamage(float a_Value)
    {
        if (m_CurHp <= 0.0f)    //��Ʈ���̰� �������� �ʰ� �Ǳ� ������ Ȥ�ó�
            return;             //������ �ι� ������ �Ǵ� ��� ����

        GameMgr.Inst.DamageText((int)a_Value, this.transform.position); //������ ��Ʈ �Լ� ȣ��

        m_CurHp -= a_Value;

        if (m_CurHp < 0.0f)
            m_CurHp = 0.0f;

        if(HpBarUI != null)
            HpBarUI.fillAmount = m_CurHp/m_MaxHp;   //���� ü�¹�UI ǥ��

        if (m_CurHp <= 0.0f) //���� ���ó��
        {
            //����

            Destroy(gameObject);    //���� ����
        }
    }
}
