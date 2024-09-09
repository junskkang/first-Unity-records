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


    private void Awake()
    {
        //cannon �������� Resources �������� �ҷ��� ������ �Ҵ�
        cannon = Resources.Load("Cannon") as GameObject;
        //��ź �߻� ���� ������ Resources �������� �ҷ��� ������ �Ҵ�
        fireSfx = Resources.Load<AudioClip>("CannonFire");
        //AudioSource ������Ʈ�� �Ҵ�
        sfx = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //���콺 ���� ��ư Ŭ���� �߻� ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    void Fire()
    {
        //�߻� ���� ���
        sfx.PlayOneShot(fireSfx, 1.0f);
        Instantiate(cannon, firePos.position, firePos.rotation);
    }
}
