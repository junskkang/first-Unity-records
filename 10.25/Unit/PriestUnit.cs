using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Priest_Att : Ally_Atrribute
{
    //������Ʈ ���� �Ӽ� �߰�

    public Priest_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Priest;
        name = "������Ʈ";
        level = 0;
        maxHp = 0;
        maxMp = 0;
        //attack = 0;

        //������Ʈ ���� �Ӽ� �⺻�� �ο�
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� ������Ʈ ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<PriestUnit>();
        //�� Priest_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class PriestUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
    }

    // Update is called once per frame
    protected override void Update()
    {
        //������Ʈ ���� �ൿ ����
    }

    public override void Attack()
    {
        //������Ʈ ���� ���� ����
    }

    public override void Skill()
    {
        //������Ʈ ���� ��ų ����
    }
}
