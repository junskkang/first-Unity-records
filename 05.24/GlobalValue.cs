using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Info  //�� Item ����
{
    public string m_Name = "";                  //ĳ���� �̸�
    public SkillType m_SkType = SkillType.Skill_0; //ĳ���� Ÿ��
    public Vector2 m_IconSize = Vector2.one;  //�������� ���� ������, ���� ������
    public int m_Price = 100;   //������ �⺻ ���� 
    public int m_UpPrice = 50; //���׷��̵� ����, Ÿ�Կ� ����
    public int m_Level = 0;
    public string m_SkillExp = "";    //��ų ȿ�� ����
    public Sprite m_IconImg = null;   //ĳ���� �����ۿ� ���� �̹���

    public void SetType(SkillType a_SkType)
    {
        m_SkType = a_SkType;

        if (a_SkType == SkillType.Skill_0)
        {
            m_Name = "������";
            m_IconSize.x = 0.766f; //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;   //���θ� �������� ���� ���̱� ������ �׳� 1.0f = 103 �ȼ�

            m_Price = 100; //�⺻����
            m_UpPrice = 50; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "Hp 20% ȸ��";
            m_IconImg = Resources.Load("IconImg/m0011", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_1)
        {
            m_Name = "�����";
            m_IconSize.x = 0.81f;    //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 200; //�⺻����
            m_UpPrice = 100; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "�ñر�";
            m_IconImg = Resources.Load("IconImg/m0367", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_2)
        {
            m_Name = "����ȣ";
            m_IconSize.x = 0.946f;     //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 400; //�⺻����
            m_UpPrice = 200; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "��ȣ��";
            m_IconImg = Resources.Load("IconImg/m0054", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_3)
        {
            m_Name = "�߿���";
            m_IconSize.x = 0.93f;     //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 800; //�⺻����
            m_UpPrice = 400; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "����ź";
            m_IconImg = Resources.Load("IconImg/m0423", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_4)
        {
            m_Name = "�巡��";
            m_IconSize.x = 0.93f;     //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 1600; //�⺻����
            m_UpPrice = 800; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "����";
            m_IconImg = Resources.Load("IconImg/m0244", typeof(Sprite)) as Sprite;
        }
        else if (a_SkType == SkillType.Skill_5)
        {
            m_Name = "��Ŀ��";
            m_IconSize.x = 0.93f;    //���ο� ���� ���� ����
            m_IconSize.y = 1.0f;     //���θ� �������� ���� ���̱� ������ �׳� 1.0f

            m_Price = 3000;   //�⺻����
            m_UpPrice = 1600; //Lv1->Lv2  (m_UpPrice + (m_UpPrice * (m_Level - 1)) ���� �ʿ�

            m_SkillExp = "��ȯ�� ����";
            m_IconImg = Resources.Load("IconImg/m0172", typeof(Sprite)) as Sprite;
        }

    }//public void SetType(SkillType a_SkType)
}

public class GlobalValue
{
    //��ȯ�� ��ų ������ ������ ����Ʈ
    public static List<Skill_Info> SkDataList = new List<Skill_Info>(); //��ų ������ ���� ����Ʈ
    public static List<int> curSkillCount = new List<int>();    //��ų ������ ���� ��

    public static string NickName = "";
    public static int bestScore = 0;
    public static int userGold = 0;

    public static void LoadGameData()
    {
        if (SkDataList.Count <= 0)
        {
            Skill_Info skillItemNd;
            for (int i = 0; i < (int)SkillType.SkCount; i++)
            {
                skillItemNd = new Skill_Info();
                skillItemNd.SetType((SkillType)i);
                SkDataList.Add(skillItemNd);
            }
        }

        //������ ����
        NickName = PlayerPrefs.GetString("NickName", "�ʾ��");
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        userGold = PlayerPrefs.GetInt("UserGold", 0);

        //������ �ε�
        if (curSkillCount.Count <= 0)
        {

            int skCount = 0;
            for(int i = 0; i < (int)SkillType.SkCount; i++)
            {
                skCount = PlayerPrefs.GetInt($"Skill_Item{i}", 1);
                curSkillCount.Add(skCount);
            }
        }
    }

}