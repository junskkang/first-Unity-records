using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    //�ҷ��������� ���ΰ��� ���͵� ����� �������� �ѹ��� �ε���Ű���� ��
    //���� �������� ����ϴµ� ���ΰ��� ���Ϳ��� ���� �ε���Ű�� ���ʿ��� ����
    public static GameObject m_BulletPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        //���ӸŴ����� �����ϸ鼭 �ҷ��������� �ε���
        m_BulletPrefab = Resources.Load("BulletPrefab") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
