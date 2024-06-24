using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ݵ�� �ʿ��� ������Ʈ�� ����� �ش� ������Ʈ�� �����Ǵ� ���� �����ϴ� Attribute
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //�Ѿ� ������
    public GameObject bullet;
    //�Ѿ� �߻���ǥ
    public Transform firePos;
    //�Ѿ� ������Ʈ Ǯ ��ġ
    public Transform bulletPool;

    //�Ѿ��� ������ƮǮ������ ������ ����Ʈ
    public List<GameObject> bulletPoolList = new List<GameObject>();

    float fireTimer = 0.0f;

    //��� �߻� ����
    public AudioClip fireSfx;
    //AudioSource ������Ʈ�� ������ ����
    private AudioSource source = null;
    //MuzzleFlash�� MeshRenderer ������Ʈ ���� ����
    public MeshRenderer muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        //AudioSource ������Ʈ�� ������ �� ������ �Ҵ�
        source = GetComponent<AudioSource>();
        //���ʿ� MuzzleFlash MeshRenderer�� ��Ȱ��ȭ
        muzzleFlash.enabled = false;

        ObjectPoolBullet();
    }

    void ObjectPoolBullet()
    {
        //�Ѿ��� �̸� ������ ������Ʈ Ǯ�� ����
        for (int i = 0; i < 50; i++)
        {
            //�Ѿ� �������� ����
            GameObject bullet = (GameObject)Instantiate(this.bullet, firePos.position, firePos.rotation);
            //������ �Ѿ��� �̸� ����
            bullet.name = "Bullet_" + i.ToString();
            //������ �Ѿ� firePos child�� ����
            bullet.transform.SetParent(bulletPool);
            //������ �Ѿ� ��Ȱ��ȭ ����
            bullet.SetActive(false);
            //������ �Ѿ��� ������Ʈ Ǯ ����Ʈ�� �߰�
            bulletPoolList.Add(bullet);
        }

        //Debug.Log(bulletPoolList.Count);
    }
    // Update is called once per frame
    void Update()
    {
        //��� ���� �� ���� �Ұ���
        if (GameManager.GameState == GameState.GameEnd) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0.0f)
        {
            fireTimer = 0.0f;

            //���콺 ���� ��ư�� Ŭ������ �� Fire �Լ� ȣ��
            if (Input.GetMouseButton(0))
            {
                if(GameManager.IsPointerOverUIObject() == false)
                    Fire();

                fireTimer = 0.11f;
            }
        }//if (fireTimer <= 0.0f)
    }

    void Fire()
    {
        //������Ʈ Ǯ ����Ʈ�� ó������ ������ ��ȸ
        foreach (GameObject bullet in bulletPoolList)
        {
            //��Ȱ��ȭ ���η� ��� ������ ���� �Ǵ�
            if (bullet.activeSelf == false)
            {
                //���� ���� �ʱ�ȭ
                bullet.transform.position = firePos.position;
                bullet.transform.rotation = firePos.rotation; //Quaternion.LookRotation(Vector3.forward, firePos.position);
                //������Ʈ Ǯ���� ����� �Ѿ� Ȱ��ȭ
                bullet.SetActive(true);
                //������Ʈ Ǯ���� �Ѿ� ������ �ϳ��� Ȱ��ȭ�� �� for ������ ��������
                break;
            }
        }


        //���� �߻� �Լ�
        source.PlayOneShot(fireSfx, 0.9f);
        //��� ��ٸ��� ��ƾ�� ���� �ڷ�ƾ �Լ��� ȣ��
        StartCoroutine(this.ShowMuzzleFlash());
    }

    void CreateBullet()
    {
        //Bullet �������� �������� ����
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    //MuzzleFlash Ȱ�� / ��Ȱ��ȭ�� ª�� �ð� ���� �ݺ�
    IEnumerator ShowMuzzleFlash()
    {
        //MuzzleFlash �������� �ұ�Ģ�ϰ� ����
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //MuzzleFlash�� Z���� �������� �ұ�Ģ�ϰ� ȸ����Ŵ
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360.0f));
        muzzleFlash.transform.localRotation = rot;

        //Ȱ��ȭ�ؼ� ���̰� ��
        muzzleFlash.enabled = true;

        //�ұ�Ģ���� �ð� ���� Delay�� ���� MeshRenderer�� ��Ȱ��ȭ
        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        //��Ȱ��ȭ�ؼ� ������ �ʰ� ��
        muzzleFlash.enabled = false;

    }//IEnumerator ShowMuzzleFlash()
}
