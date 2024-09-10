using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCannon : MonoBehaviour
{
    //Canoon �������� ������ ����
    public GameObject cannon = null;
    //Cannon �߻� ����
    public Transform firePos;

    //��ź �߻� ���� ����
    private AudioClip fireSfx = null;
    //AudioSource ������Ʈ�� �Ҵ��� ����
    private AudioSource sfx = null;

    private PhotonView pv = null;


    private void Awake()
    {
        //cannon �������� Resources �������� �ҷ��� ������ �Ҵ�
        cannon = Resources.Load("Cannon") as GameObject;
        //��ź �߻� ���� ������ Resources �������� �ҷ��� ������ �Ҵ�
        fireSfx = Resources.Load<AudioClip>("CannonFire");
        //AudioSource ������Ʈ�� �Ҵ�
        sfx = GetComponent<AudioSource>();

        pv = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���콺 ���� ��ư Ŭ���� �߻� ���� ����
        if (pv.IsMine && Input.GetMouseButtonDown(0))
        {
            //�ڽ��� ��ũ�� ���� �����Լ��� ȣ���� ��ź�� �߻�
            Fire();
            //���� ��Ʈ��ũ�� �ִ� �÷��̾��� ��ũ�� RPC�� �������� �Լ��� ȣ��
            pv.RPC("Fire", RpcTarget.Others, null);

            //�ڽŰ� ��� ��Ʈ��ũ �÷��̾�� �Լ�ȣ��
            //pv.RPC("Fire", RpcTarget.All, null);

            //Bufferd : ���߿� ������ �÷��̾�� ���ۿ� ����� ������ �޷�� ������
            //������ ���� ��� ���� ������ ���ۿ� ���� �ʰ� �ٷ� ������
            //ex) ��� ä��â. ���� �����ϱ� �� ������� �� ��
            //All : ���� ��� / �ٸ� ������� RPC�� ���ؼ�
            //����Ʈ ���� ������ �ٷ� ǥ���Ͽ� �������� ���̰�, ����ȭ�� �ӵ��� ������� ���̰� ����
            //AllViaServer : ���� ������ ��ο��� RPC�� ���ؼ� �߰�����
            //������ �������� ����ȭ�� ��Ȯ���� ����
        }
    }

    [PunRPC]
    void Fire()
    {
        //�߻� ���� ���
        sfx.PlayOneShot(fireSfx, 1.0f);
        Instantiate(cannon, firePos.position, firePos.rotation);
    }
}
