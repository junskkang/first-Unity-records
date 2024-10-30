using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Priest_Att : Ally_Atrribute
{
    //프리스트 고유 속성 추가

    public Priest_Att()    //생성자 오버로딩 함수
    {
        //아군 공통 속성 기본값 부여
        type = AllyType.Priest;
        name = "프리스트";
        level = 0;
        maxHp = 0;
        maxMp = 0;
        //attack = 0;

        //프리스트 고유 속성 기본값 부여
    }

    //Ally게임 오브젝트에 빙의시킬 Ally클래스를 추가해주는 함수
    public override AllyUnit MyAddComponent(GameObject parentObject)
    {
        //매개변수로 받은 부모 오브젝트에 프리스트 컴포넌트를 붙여줌
        AllyUnit a_RefAlly = parentObject.AddComponent<PriestUnit>();
        //이 Priest_Att 객체를 AllyUnit 쪽의 ally_Attribute 변수에 대입해준다
        a_RefAlly.ally_Attribute = this;

        return a_RefAlly;
    }
}

public class PriestUnit : AllyUnit
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();  //부모쪽 Start() 호출 (공통 등장 준비)
    }

    // Update is called once per frame
    protected override void Update()
    {
        //프리스트 고유 행동 패턴
    }

    public override void Attack()
    {
        //프리스트 고유 공격 패턴
    }

    public override void Skill()
    {
        //프리스트 고유 스킬 패턴
    }
}
