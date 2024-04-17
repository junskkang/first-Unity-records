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
            //마우스 클릭 하는 방향으로 레이저 광선을 쏨
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity))   //레이저 빛을 무한대로 쏘겠다는 의미
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
            //사운드가 중복되어서 발동하였을 때
            //Play : 앞의 사운드가 그 즉시 멈추고 마지막에 발동된 것이 재생됨
            //PlayOneShot : 사운드가 온전히 재생됨. = 중복 재생 된다는 의미
        }
        else if (other.gameObject.tag == "Bomb")
        {
            GameManager.Inst.GetBomb();
            this.aud.PlayOneShot(this.bombSE);
        }
        Destroy(other.gameObject);
    }
}
