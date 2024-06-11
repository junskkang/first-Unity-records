using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ŭ������ System.Serializable�̶�� ��Ʈ����Ʈ(Attribute)�� ����ؾ�
//Inspector �信 �����
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
    //ĳ���� �̵� ���� ����
    private float h = 0.0f;
    private float v = 0.0f;
    
    public float moveSpeed = 10.0f; //�̵��ӵ� ����
    public float rotSpeed = 100.0f; //ȸ������

    //�ν����ͺ信 ǥ���� �ִϸ��̼� Ŭ���� ����
    public Anim anim;

    //�Ʒ��� �ִ� 3D���� Animation ������Ʈ�� �����ϱ� ���� ����
    public Animation _animation;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        //�ڽ��� ������ �ִ� Animation ������Ʈ�� ã�ƿ� ������ �Ҵ�
        _animation = GetComponentInChildren<Animation>();

        //Animation ������Ʈ�� �ִϸ��̼� Ŭ���� �����ϰ� ����
        _animation.clip = anim.idle;
        _animation.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //�����¿� �̵� ���� ���� ���
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        if (1.0f < moveDir.magnitude)   //�밢�� �������� ����
            moveDir.Normalize();

        //Translate(�̵����� * �ӵ� * ������ * Time.deltaTime, ������ǥ)
        //Vector3.forward = Space.Self
        //transform.forward = Space.World
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        //Vector3.up ���� �������� rotSpeed��ŭ�� �ӵ��� ȸ��
        transform.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"));

        //Ű���� �Է°��� �������� ������ �ִϸ��̼� ����
        if (v >= 0.1f)
        {
            //���� �ִϸ��̼�
            _animation.CrossFade(anim.runForward.name, 0.3f);
            //anim.runForward.name : anim������ ����� �ִϸ��̼��� �̸��� �ҷ����� �ڵ�
            //_animation.CrossFade("runForward", 0.3f); �̷��� �ᵵ �Ȱ��� ���
            //0.3f�� ���� �ð� �ִϸ��̼� ��ȯ fadeȿ��. ����Ʈ �� == 0.3f
            //_animation.Play(runFoward);�ε� �����ų �� �ִµ� ��� �����ð��� ����
        }
        else if (v <= -0.1f)
        {
            //���� �ִϸ��̼�
            _animation.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f)
        {
            //������ �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f)
        {
            //���� �̵� �ִϸ��̼�
            _animation.CrossFade(anim.runLeft.name, 0.3f);
        }
        else
        {
            //������ idle �ִϸ��̼�
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }
}
