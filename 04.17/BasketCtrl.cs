using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketCtrl : MonoBehaviour
{
    [Header("Audio Clip")]
    public AudioClip appleSE;
    public AudioClip bombSE;
    AudioSource aud;

    Rigidbody rigid;
    ParticleSystem particleSystem;
    
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        this.aud = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        particleSystem = GetComponent<ParticleSystem>();
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) 
        { 
            //���콺 Ŭ�� �ϴ� �������� ������ ������ ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))   //������ ���� ���Ѵ�� ��ڴٴ� �ǹ�
            { 
                float x = Mathf.RoundToInt(hit.point.x);
                float z = Mathf.RoundToInt(hit.point.z);
                transform.position = new Vector3(x, 0, z);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Apple")
        {
            GameManager.Inst.GetApple();
            this.aud.PlayOneShot(this.appleSE);
            particleSystem.Play();
            //���尡 �ߺ��Ǿ �ߵ��Ͽ��� ��
            //Play : ���� ���尡 �� ��� ���߰� �������� �ߵ��� ���� �����
            //PlayOneShot : ���尡 ������ �����. = �ߺ� ��� �ȴٴ� �ǹ�
        }
        else if (other.gameObject.tag == "Bomb")
        {
            GameManager.Inst.GetBomb();
            this.aud.PlayOneShot(this.bombSE);
        }
        Destroy(other.gameObject);
    }
}
