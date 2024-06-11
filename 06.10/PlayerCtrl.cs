using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스에 System.Serializable이라는 어트리뷰트(Attribute)를 명시해야
//Inspector 뷰에 노출됨
[System.Serializable]

public class Anim
{
    public AnimationClip idle;
    public AnimationClip runForward;
    public AnimationClip runBackward;
    public AnimationClip runRight;
    public AnimationClip runLeft;

}
public class PlayerCtrl : MonoBehaviour
{
    //캐릭터 이동 관련 변수
    private float h = 0.0f;
    private float v = 0.0f;
    
    public float moveSpeed = 10.0f; //이동속도 변수
    public float rotSpeed = 100.0f; //회전변수

    //인스펙터뷰에 표시할 애니메이션 클래스 변수
    public Anim anim;

    //아래에 있는 3D모델의 Animation 컴포넌트에 접근하기 위한 변수
    public Animation _animation;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //자신의 하위에 있는 Animation 컴포넌트를 찾아와 변수에 할당
        _animation = GetComponentInChildren<Animation>();

        //Animation 컴포넌트의 애니메이션 클립을 지정하고 실행
        _animation.clip = anim.idle;
        _animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)   //대각선 단위벡터 제한
            moveDir.Normalize();

        //Translate(이동방향 * 속도 * 변위값 * Time.deltaTime, 기준좌표)
        //Vector3.forward = Space.Self
        //transform.forward = Space.World
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        //Vector3.up 축을 기준으로 rotSpeed만큼의 속도로 회전
        transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        //키보드 입력값을 기준으로 동작할 애니메이션 수행
        if (v >= 0.1f)
        {
            //전진 애니메이션
            _animation.CrossFade(anim.runForward.name, 0.3f);
            //anim.runForward.name : anim변수에 연결된 애니메이션의 이름을 불러오는 코드
            //_animation.CrossFade("runForward", 0.3f); 이렇게 써도 똑같은 결과
            //0.3f은 보간 시간 애니메이션 전환 fade효과. 디폴트 값 == 0.3f
            //_animation.Play(runFoward);로도 재생시킬 수 있는데 대신 보간시간이 없음
        }
        else if (v <= -0.1f)
        {
            //후진 애니메이션
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            //오른쪽 이동 애니메이션
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            //왼쪽 이동 애니메이션
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            //정지시 idle 애니메이션
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }
}
