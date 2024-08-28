using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MarkMgr : MonoBehaviour
{
    public Transform refAimObj;
    float Speed = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        if(refAimObj == null )
            refAimObj.GetComponent<Transform>();

        Speed = GameMgr.inst.downscaleSpeed;

        Invoke("Miss", 6.5f/ Speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (refAimObj != null)
        {
            refAimObj.transform.Rotate(0, 0, 10 * Time.deltaTime);
            refAimObj.transform.localScale = new Vector3(refAimObj.transform.localScale.x - Speed * Time.deltaTime,
                refAimObj.transform.localScale.y - Speed * Time.deltaTime, refAimObj.transform.localScale.z);
        }

        if (refAimObj.transform.localScale.x <= 0.85f)
        {
            refAimObj.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }

    void Miss()
    {
        Destroy(this.gameObject);
        GameMgr.inst.Judge("Miss", gameObject.transform.position);
    }

    public void HitPointerCheck()
    {
        string hit = "";
        
        if (4.25f <= refAimObj.transform.localScale.x)
            hit = "Miss";
        else if (3.05f <= refAimObj.transform.localScale.x)
            hit = "Good";
        else if (1.95f <= refAimObj.transform.localScale.x)
            hit = "Great";
        else if (0.85f <= refAimObj.transform.localScale.x)
            hit = "Excellent";
        else if (refAimObj.transform.localScale.x <= 0.85f)
            hit = "Nice";

        Destroy(this.gameObject);
        GameMgr.inst.Judge(hit, gameObject.transform.position);
    }
}
