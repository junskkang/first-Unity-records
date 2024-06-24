using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //������ Ÿ�� ���ӿ�����Ʈ�� Transform ����
    public float dist = 10.0f;      //ī�޶���� ���� �Ÿ�
    public float height = 3.0f;     //ī�޶��� ���� ����
    public float dampTrace = 20.0f; //�ε巯�� ������ ���� ����

    Vector3 m_PlayerVec = Vector3.zero;
    float rotSpeed = 10.0f;

    public bool isBorder = false;
    public Material materials;
    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
    }

    void Update()
    {
        //��� ���� �� ���� �Ұ���
        if (GameManager.GameState == GameState.GameEnd) return;

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameManager.IsPointerOverUIObject() == false)
        {
            //--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
            height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));  

            if (height < 0.1f)
                height = 0.1f;

            if (5.7f < height)
                height = 5.7f;
            //--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
        }

        StopToWall();

    }//void Update()

    //Update �Լ� ȣ�� ���� �� ���� ȣ��Ǵ� �Լ��� LateUpdate ���
    //������ Ÿ���� �̵��� ����� ���Ŀ� ī�޶� �����ϱ� ���� LateUpdate ���
    // Update is called once per frame
    void LateUpdate()
    {
        switch (GameManager.playerCharacter)
        {
            case PlayerCharacter.Player1:
                FollowPlayer(GameManager.inst.player1.transform);
                break;
            case PlayerCharacter.Player2:
                FollowPlayer(GameManager.inst.player2.transform);
                break;
        }
    }

    void FollowPlayer(Transform playerTr)
    {
        m_PlayerVec = playerTr.position;
        m_PlayerVec.y += 1.2f;

        //ī�޶��� ��ġ�� ��������� dist ������ŭ �������� ��ġ�ϰ�
        //height ������ŭ ���� �ø�
        //Lerp�Լ� �ѹ��� ���� �����ϴ°� �ƴ϶� �������� �����ϵ��� �ϴ� �Լ�
        //ī�޶� ��ġ�� �ް��ϰ� �������� �ѹ��� ��~ �ϰ��ϴ°� �ƴ϶� �̵��ϴ� ȿ���� �� �� ����
        transform.position = Vector3.Lerp(transform.position,
                                            playerTr.position
                                            - (playerTr.forward * dist)
                                            + (Vector3.up * height),
                                            Time.deltaTime * dampTrace);

        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.LookAt(m_PlayerVec);
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 3.2f, Color.green);  //���� ��ġ, ���� * ����, ��
        RaycastHit hit;
        
        isBorder = Physics.Raycast(transform.position, transform.forward, 
                  out hit, 3.2f, LayerMask.GetMask("Wall"));
        //Raycast(������ġ, ����, ����, ���̾��ũ)
        //�ش� ����ĳ��Ʈ�� wall�̶�� ���̾��ũ�� ������ true���� ��ȯ

        if (isBorder)
        {
            //Debug.Log(hit.collider.name);
            hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
        }
        
    }
}
