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

    //������ȭ �� Ǯ��
    LayerMask wallMask = -1;
    List<WallCtrl> wallList = new List<WallCtrl>();
    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;

        ////Wall ����Ʈ �����
        //wallMask = 1 << LayerMask.NameToLayer("SideWall");
        ////SideWall ���̾ layüũ�ϱ� ���� ����ũ ���� ����

        //GameObject[] sideWalls = GameObject.FindGameObjectsWithTag("SideWall");
        //for (int i = 0; i < sideWalls.Length; i++)
        //{
        //    WallCtrl wallCtrl = sideWalls[i].GetComponent<WallCtrl>();
        //    wallCtrl.isColl = false;
        //    wallCtrl.WallAlphaOnOff(false); //������ȭ�� ����
        //    wallList.Add(wallCtrl);
        //}
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

        //StopToWall();

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

        StopToWall();
    }

    void StopToWall()
    {
        float dist = (transform.position - m_PlayerVec).magnitude;

        Vector3 toCamera = transform.position - m_PlayerVec;
        toCamera.Normalize();

        Debug.DrawRay(m_PlayerVec, toCamera * dist, Color.green);  //���� ��ġ, ���� * ����, ��
        RaycastHit hit;
        

        //����ĳ��Ʈ�� �� �� ��!
        //ī�޶󿡼� ĳ���͸� ���� ��� �ͺ��� ĳ���Ϳ��� ī�޶� ���� ��� ���� ������ ����
        //��? ���� �β��� �����ϰ� ���� ���δ� ������� ��� raycast�� hit���� �ʾ�
        //ī�޶� ��ġ�� ���� ���ο� ������ ���� �������� ���Ѵٴ� �ǹ�
        //ī�޶��� ��ġ�� ������ ���� ����� �׶����� hit�� �Ǳ� �����ϱ� ������
        //���� �β��� ��ٸ� ũ�� ���̴� �������� ���� �β������� �β���������
        //�� ���̰� �и��� ��. ���� ĳ���Ϳ������� ī�޶� ���� ���
        //��� ������Ʈ�� ������� �Ѵ��ص� Ȯ���ϰ� ó���� �� ����

        isBorder = Physics.Raycast(m_PlayerVec, toCamera, 
                  out hit, dist, LayerMask.GetMask("Wall"));
        //Raycast(������ġ, ����, ����, ���̾��ũ)
        //�ش� ����ĳ��Ʈ�� wall�̶�� ���̾��ũ�� ������ true���� ��ȯ

        if (isBorder)
        {
            //Debug.Log(hit.collider.name);
            hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
        }
        
    }
}
