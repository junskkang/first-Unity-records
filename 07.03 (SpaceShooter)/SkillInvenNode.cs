using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInvenNode : MonoBehaviour
{
    [HideInInspector] public SkillType type;
    [HideInInspector] public Text skCountText;

    void Start()
    {
        Button component = this.GetComponent<Button>();
        if (component != null)
            component.onClick.AddListener(() =>
            {//�� ��ư�� ������ �� �ش� ��ų�� ���ǰԲ�
                if (GlobalValue.g_SkillCount[(int)type] <= 0) return;

                PlayerCtrl player = GameObject.FindObjectOfType<PlayerCtrl>();
                if (player != null )
                    player.UseSkill_Item(type);

                if(skCountText != null )
                    skCountText.text = GlobalValue.g_SkillCount[(int)this.type].ToString();
            });
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    public void InitState(SkillType type)
    {
        this.type = type;
        skCountText = GetComponentInChildren<Text>();

        if(skCountText != null )
            skCountText.text = GlobalValue.g_SkillCount[(int)this.type].ToString();
    }
}
