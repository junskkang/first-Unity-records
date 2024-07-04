using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;      //������ Ÿ�� ���ӿ�����Ʈ�� Transform ����
    FireCtrl fireCtrl = null;       //������ Ÿ���� ���� �ִ� fireCtrl��ũ��Ʈ ���� ���� 
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

    //ī�޶� ��ġ ���� ����
    float rotV = 0.0f;          //���콺 ���� ���۰� ���� ����
    float defaultRotV = 25.2f;  //���� ������ ȸ������, ���밭�� ������� ���� �� ī�޶��� �ʱ�ȸ������ x = 25.2��
    float marginRotV = 22.3f;   //�ѱ����� ���� ���� : firePos�� ī�޶��� ������ ��
    float minLimitV = -17.9f;   //�� �Ʒ� ���� ����
    float maxLimitV = 52.9f;    //�� �Ʒ� ���� ����
    float maxDist = 4.0f;       //���콺 �� �ƿ� �ִ�Ÿ� ���� 
    float minDist = 0.1f;       //���콺 �� �� �ִ� �Ÿ� ����
    float zoomSpeed = 0.7f;      //���콺 �� ���ۿ� ���� �� �� �� �ƿ� ���ǵ� ��

    Quaternion buffRot;         //ī�޶� ȸ�� ���� ����
    Vector3 buffPos;            //ī�޶� ȸ���� ���� ��ġ ��ǥ ���� ����
    Vector3 basicPos = Vector3.zero; //��ġ ���� ����
    float saveDist;

    //�� ���� ���� ���� ����
    public static Vector3 riffleDir = Vector3.zero;     //�� ���� ����
    Quaternion cacRFRot;
    Vector3 cacRFPos = Vector3.forward;

    // Start is called before the first frame update
    void Start()
    {
        dist = 3.4f;
        height = 2.8f;
        saveDist = dist;

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

        //ī�޶� ��ġ ���
        rotV = defaultRotV;
    }

    void Update()
    {
        //��� ���� �� ���� �Ұ���
        if (GameManager.GameState == GameState.GameEnd) return;

        if (Input.GetMouseButton(0) == true || Input.GetMouseButton(1) == true)
        if (GameManager.IsPointerOverUIObject() == false)
        {
            //���� ���
            ////--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�
            //height -= (rotSpeed * Time.deltaTime * Input.GetAxis("Mouse Y"));  

            //if (height < 0.1f)
            //    height = 0.1f;

            //if (5.7f < height)
            //    height = 5.7f;
            ////--- ī�޶� �� �Ʒ� �ٶ󺸴� ���� ������ ���� ������ ���� �ڵ�


            //������ǥ�踦 �̿��� �Ź�� 24.07.03
            float addRotSpeed = 235.0f;     //Ʃ�װ�
            rotSpeed = addRotSpeed;
            rotV -= (rotSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse Y"));
            if(rotV < minLimitV)
                rotV = minLimitV;
            if(maxLimitV <rotV)
                rotV = maxLimitV;
        }

        //ī�޶� ���� �ܾƿ�
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && dist < maxDist)
        {
            dist += zoomSpeed;
            saveDist = this.dist;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && minDist < dist)
        {
            dist -= zoomSpeed;
            saveDist = this.dist;
        }       

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

        //ī�޶� ��ġ ����ִ� ���밭�� �ҽ� (������ǥ�� �̿�)
        //ī�޶��� ��ġ�� ��������� dist ������ŭ �������� ��ġ�ϰ�
        //height ������ŭ ���� �ø�
        //Lerp�Լ� �ѹ��� ���� �����ϴ°� �ƴ϶� �������� �����ϵ��� �ϴ� �Լ�
        //ī�޶� ��ġ�� �ް��ϰ� �������� �ѹ��� ��~ �ϰ��ϴ°� �ƴ϶� �̵��ϴ� ȿ���� �� �� ����
        //transform.position = Vector3.Lerp(transform.position,
        //                                    playerTr.position
        //                                    - (playerTr.forward * dist)
        //                                    + (Vector3.up * height),
        //                                    Time.deltaTime * dampTrace);

        //������ǥ�踦 ������ǥ��� ȯ���Ͽ� ī�޶� ��ġ�� ����ִ� �ҽ� 24.07.03

        buffRot = Quaternion.Euler(rotV, targetTr.eulerAngles.y, 0.0f);
        //ù��° �Ű������� x�࿡�ٰ� ȸ������ �־� ���Ʒ��� �����̵��� ��
        //�ι�° �Ű��������� y���� ȸ�����ε� ī�޶��� Ÿ��(�÷��̾�)�� �ٶ󺸰� �ִ� ȸ�� ������ ������
        basicPos = new Vector3(0.0f, 0.0f, -dist);
        //basicPos�� dist�Ÿ� ��ŭ �ڸ� ���ϴ� ����, ������ �Ÿ��� �����ϵ��� �ϴ� ���� ����� ����
        buffPos = m_PlayerVec + (buffRot * basicPos);
        //Vector3���ٰ� Quaternion�� ���ϸ� Quaternion�� ȸ�������� ����� Vector3�� ������ ��.
        //��� buffRot ������ ���ϴ� basicPos�� z��(-dist)�Ÿ���ŭ
        //������ Vector3�� buffPos�� �Ǵ� ��
        transform.position = Vector3.Lerp(transform.position, buffPos, Time.deltaTime * dampTrace);
        
        //ī�޶� Ÿ�� ���ӿ�����Ʈ�� �ٶ󺸰� ����
        transform.LookAt(m_PlayerVec);

        //�ѱ��� ���Ʒ� ���� ���
        if (fireCtrl == null)
            fireCtrl = targetTr.GetComponent<FireCtrl>();

        Vector3 cPos = Vector3.zero;
        if (rotV < 6.0f) 
        {
            cPos = fireCtrl.firePos.localPosition;
            cPos.y = 1.53f;
            fireCtrl.firePos.localPosition = cPos;
        }
        else
        {
            cPos = fireCtrl.firePos.localPosition;
            cPos.y = 1.41f;
            fireCtrl.firePos.localPosition = cPos;
        }


        cacRFRot = Quaternion.Euler(Camera.main.transform.eulerAngles.x - marginRotV,
                                    targetTr.eulerAngles.y, 0.0f);
        riffleDir = cacRFRot * cacRFPos;
        //ī�޶� �ٶ󺸴� ������ cacRFPos(forward���溤��)���ٰ� �����־� 
        //ī�޶� �ٶ󺸴� �������� riffleDir�� ������
        //�ʱ��� ī�޶�� firePos�� ������ ������ ä ȸ����Ű�� ���� margin������ ����



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
            transform.position = hit.point;

            //float collDist = (transform.position - m_PlayerVec).magnitude;
            ////Debug.Log(hit.collider.name);
            ////hit.collider.gameObject.GetComponent<MeshRenderer>().material = materials;
            //if (collDist > 0.85f)
            //    this.dist = collDist - 0.3f;            
        }
        else if (!isBorder)
        {
            this.dist = Mathf.Lerp(this.dist, saveDist, Time.deltaTime * zoomSpeed * 2);
        }
    }
}
