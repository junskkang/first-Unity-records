using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    //������ ���� ����
    public GameObject laserDot;
    LayerMask laserMask = -1;

    //����ź ����
    public GameObject grenadePrefab;
    // Start is called before the first frame update
    void Start()
    {
        //AudioSource ������Ʈ�� ������ �� ������ �Ҵ�
        source = GetComponent<AudioSource>();
        //���ʿ� MuzzleFlash MeshRenderer�� ��Ȱ��ȭ
        muzzleFlash.enabled = false;

        ObjectPoolBullet();

        laserMask = 1 << LayerMask.NameToLayer("BULLET");
        laserMask |= 1 << LayerMask.NameToLayer("E_BULLET");
        laserMask = ~laserMask;     //~�� �ǹ̴� ������ �ǹ�. ���� ������ ���̾ ���ܽ�Ű�� ���� ��.
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
                    Fire(BulletType.BULLET, firePos);

                fireTimer = 0.11f;
            }
        }//if (fireTimer <= 0.0f)
         //ī�޶� �ٶ󺸰� �ִ� �������� �ѱ��� Ʋ� ���� �������� �Ѿ��� ���ư����� ��
        firePos.transform.forward = FollowCam.riffleDir.normalized;

        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    FireGrenade();
        //}

        LaserAim();
    }

    public void Fire(BulletType type, Transform typeVector)
    {
        //������Ʈ Ǯ ����Ʈ�� ó������ ������ ��ȸ
        foreach (GameObject bullet in bulletPoolList)
        {
            //��Ȱ��ȭ ���η� ��� ������ �Ѿ� �Ǵ�
            if (bullet.activeSelf == false)
            {
                bullet.gameObject.tag = type.ToString();               
                //���� ���� �ʱ�ȭ
                bullet.transform.position = typeVector.position;
                bullet.transform.rotation = typeVector.rotation; //Quaternion.LookRotation(Vector3.forward, firePos.position);
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

    void LaserAim()
    {
        Debug.DrawRay(firePos.position, firePos.transform.forward * 15.0f, Color.green);  //���� ��ġ, ���� * ����, ��
        RaycastHit hit;

        if (Physics.Raycast(firePos.position, firePos.transform.forward, out hit, 60.0f, laserMask))
        {
            if (hit.collider.tag == "BULLET") return;

            if (laserDot != null)
            {                
                //Vector3 forHere = hit.point;
                //forHere.z -= 0.5f;
                //laserDot.transform.position = forHere;

                float dist = Vector3.Distance(firePos.position, hit.point);
                //Vector3 maxSize = new Vector3(0.5f, 0.5f);
                //Vector3 middleSize = new Vector3(0.3f, 0.3f);
                //Vector3 minSize = new Vector3(0.1f, 0.1f);

                float cacScale = dist * 0.3f;   //��Ÿ��� 60���� �����صּ� �ִ� 18
                if (cacScale < 1.5f)
                    cacScale = 1.5f;
                if (5.0f < cacScale)
                    cacScale = 5.0f;    //�Ÿ��� ���� ������ ��Ʈ ũ�� ���

                float cacOffset = dist / 30.0f; //��Ÿ��� 60���� �����صּ� �ִ� 2
                if (cacOffset < 0.1f)
                    cacOffset = 0.1f;
                if (0.5f < cacOffset)
                    cacOffset = 0.5f;   //�Ÿ��� ���� ������ ����Ʈ�� �� �Ÿ� ���

                laserDot.transform.position = hit.point - (firePos.transform.forward * cacOffset);
                laserDot.transform.localScale = new Vector3(cacScale, cacScale, cacScale);

                //if (dist > 10.0f)
                //{
                //    laserDot.transform.localScale = Vector3.Lerp(maxSize, minSize, Time.deltaTime * 0.001f);
                //}
                //else //if (5.0f < dist && dist < 10.0f )
                //{
                //    laserDot.transform.localScale = Vector3.Lerp(minSize, maxSize, Time.deltaTime * 0.001f);
                //}
                
                laserDot.transform.LookAt(Camera.main.transform.position);
            }                           
        }
        else //60���� �̻� ��� ��
        {
            laserDot.transform.position = firePos.position + firePos.transform.forward * 59.0f;
            laserDot.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
            laserDot.transform.LookAt(Camera.main.transform.position);  //������ ó��
        }


        //����ĳ��Ʈ�� �� �� ��!
        //ī�޶󿡼� ĳ���͸� ���� ��� �ͺ��� ĳ���Ϳ��� ī�޶� ���� ��� ���� ������ ����
        //��? ���� �β��� �����ϰ� ���� ���δ� ������� ��� raycast�� hit���� �ʾ�
        //ī�޶� ��ġ�� ���� ���ο� ������ ���� �������� ���Ѵٴ� �ǹ�
        //ī�޶��� ��ġ�� ������ ���� ����� �׶����� hit�� �Ǳ� �����ϱ� ������
        //���� �β��� ��ٸ� ũ�� ���̴� �������� ���� �β������� �β���������
        //�� ���̰� �и��� ��. ���� ĳ���Ϳ������� ī�޶� ���� ���
        //��� ������Ʈ�� ������� �Ѵ��ص� Ȯ���ϰ� ó���� �� ����

        //isBorder = Physics.Raycast(m_PlayerVec, toCamera,
        //          out hit, dist, LayerMask.GetMask("Wall"));
        //Raycast(������ġ, ����, ����, ���̾��ũ)
        //�ش� ����ĳ��Ʈ�� wall�̶�� ���̾��ũ�� ������ true���� ��ȯ
    }

    public void FireGrenade()
    {
        GameObject go = Instantiate(grenadePrefab);
        if (go != null)
        {
            go.transform.position = firePos.transform.position;
            go.GetComponent<GrenadeCtrl>().SetForwardDir(firePos.transform.forward);
        }

    }
}
