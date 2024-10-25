using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Dancer_Att : Ally_Atrribute
{
    //�� ���� �Ӽ� �߰�

    public Dancer_Att()    //������ �����ε� �Լ�
    {
        //�Ʊ� ���� �Ӽ� �⺻�� �ο�
        type = AllyType.Dancer;
        name = "��";
        level = 0;
        maxHp = 0;
        maxMp = 0;
        attack = 0;

        //�� ���� �Ӽ� �⺻�� �ο�
    }

    //Ally���� ������Ʈ�� ���ǽ�ų AllyŬ������ �߰����ִ� �Լ�
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //�Ű������� ���� �θ� ������Ʈ�� �� ������Ʈ�� �ٿ���
        AllyUnit a_RefAlly = parentObject.AddComponent<DancerUnit>();
        //�� Dancer_Att ��ü�� AllyUnit ���� ally_Attribute ������ �������ش�
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class DancerUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //�θ��� Start() ȣ�� (���� ���� �غ�)
    }

    // Update is called once per frame
    protected override void Update()
    {
        //�� ���� �ൿ ����
    }

    public override void Attack()
    {
        //�� ���� ���� ����
    }

    public override void UseSkill()
    {
        //�� ���� ��ų ����
    }
}
