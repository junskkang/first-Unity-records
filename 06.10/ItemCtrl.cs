using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCtrl : MonoBehaviour
{
    public AnimationCurve moveCurve = new AnimationCurve();

    float startTime = 0.0f;
    float curTime = 0.0f;

    float moveOffset = 0.0f;
    Vector3 moveVector = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        Destroy(gameObject, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        curTime = Time.time;    

        moveOffset = moveCurve.Evaluate(curTime - startTime);
        moveVector.y = moveOffset;
        transform.localPosition = moveVector;

        transform.Rotate(0.0f, Time.deltaTime * 200.0f, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //점수 획득
            GameManager.inst.DispGold(10);

            //오브젝트 삭제
            Destroy(gameObject);
        }
    }
}
