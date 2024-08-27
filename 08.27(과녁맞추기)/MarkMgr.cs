using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarkMgr : MonoBehaviour
{
    public Transform refAimObj;
    [HideInInspector] public float downscaleSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if(refAimObj == null )
            refAimObj.GetComponent<Transform>();

        Invoke("Miss", 2.5f/downscaleSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (refAimObj != null)
        {
            refAimObj.transform.Rotate(0, 0, 10 * Time.deltaTime);
            refAimObj.transform.localScale = new Vector3(refAimObj.transform.localScale.x - downscaleSpeed * Time.deltaTime,
                refAimObj.transform.localScale.y - downscaleSpeed * Time.deltaTime, refAimObj.transform.localScale.z);
        }
    }

    void Miss()
    {
        Destroy(this.gameObject);
        GameMgr.inst.Judge("Miss", gameObject.transform.position);
    }

    public void HitPointerCheck()
    {
        //Debug.Log("함수 호출 여부");
        if (1.41f <= refAimObj.transform.localScale.x)
        {
            Destroy(this.gameObject);
            GameMgr.inst.Judge("Miss", gameObject.transform.position);
            //게임매니저에서 점수 호출
            //넘겨줄 것 : 마크오브젝트의 위치(해당 오브젝트에 파티클 이펙트 뿌리기)
            //            스트링이나 점수를 넘겨서 계산 산정하기
            //            
        }
        else if (1.06f <= refAimObj.transform.localScale.x)
        {
            Destroy(this.gameObject);
            GameMgr.inst.Judge("Good", gameObject.transform.position);
        }
        else if (0.66f <= refAimObj.transform.localScale.x)
        {
            Destroy(this.gameObject);
            GameMgr.inst.Judge("Great", gameObject.transform.position);
        }
        else if (0.28f <= refAimObj.transform.localScale.x)
        {
            Destroy(this.gameObject);
            GameMgr.inst.Judge("Excellent", gameObject.transform.position);
        }
        else if (refAimObj.transform.localScale.x <= 0.28f)
        {
            Destroy(this.gameObject);
            GameMgr.inst.Judge("Nice", gameObject.transform.position);
        }

    }
}
