using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Hunter_Att : Ally_Atrribute
{
    //���� ���� �Ӽ� �߰�

    public Hunter_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Hunter;
        name = "����";
        level = 0;
        maxHp = 0;
        maxMp = 0;
        attack = 0;

        //���� ���� �Ӽ� �⺻�� �ο�
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� ���� ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<HunterUnit>();
        //�� Hunter_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class HunterUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
    }

    // Update is called once per frame
    protected override void Update()
    {
        //���� ���� �ൿ ����
    }

    public override void Attack()
    {
        //���� ���� ���� ����
    }

    public override void UseSkill()
    {
        //���� ���� ��ų ����
    }
}
