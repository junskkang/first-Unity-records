using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue           //�۷ι������� ���� �ؾ��ϴ� ������
{
    //������ ����
    public static string g_Unique_ID = "";        //������ ������ȣ
    public static string g_NickName = "";         //������ ����
    public static int g_BestScore = 0;            //��������
    public static int g_UserGold = 0;            //���ӸӴ�
    public static int g_Exp = 0;                  //����ġ
    public static int g_Level = 0;                //����

    public static Chr_Stat g_CurSelCStat = null;  //���� �����ϰ� �ִ� ĳ���� �ε���
    public static List<Chr_Stat> g_MyChrList = new List<Chr_Stat>(); //���� �����ϰ� �ִ� ĳ���� ����Ʈ

    public static void LoadGameData()
    {
        if (g_MyChrList.Count <= 0)   //�ε��� �ѹ��� �ǰ� �ϱ� ���ؼ�
        {
            //�� �κ��丮�� �����ϰ� �ִ� ĳ���� ��� �ε� �� �߰�
            Chr_Stat a_CrNode = new Wizard_Stat("������", 70);
            g_MyChrList.Add(a_CrNode);

            a_CrNode = new Barbarian_Stat("����ŷ", 10, 50);
            g_MyChrList.Add(a_CrNode );

            a_CrNode = new Archer_Stat("�����", 30);
            g_MyChrList.Add(a_CrNode);

            a_CrNode = new Healer_Stat("�ǿ���", 20, 60);
            g_MyChrList.Add (a_CrNode );

        }
    }
}
