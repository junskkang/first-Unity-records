using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageCtrl : MonoBehaviour
{
    public AnimationCurve scaleCurve = new AnimationCurve();
    public AnimationCurve moveCurve = new AnimationCurve();
    public AnimationCurve alphaCurve = new AnimationCurve();

    public Text refText = null;
    Color textColor;
    int damageValue = 0;

    float startTime = 0.0f;
    float curTime = 0.0f;

    Vector3 cacScaleVector = Vector3.zero;
    float cacScale = 0.0f;
    Vector3 cacMoveVector = Vector3.zero;
    float moveOffset = 0.0f;
    float colorAlpha = 1.0f;

    
    void Start()
    {
        if(refText == null)
            refText = this.gameObject.GetComponentInChildren<Text>();

        if (refText != null)
        {
            //textColor = refText.color;
            refText.text = "-" + damageValue.ToString();
        }

        startTime = Time.time;

        Destroy(this.gameObject, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        curTime = Time.time;

        cacScale = scaleCurve.Evaluate(curTime - startTime);
        cacScaleVector.x = cacScale;
        cacScaleVector.y = cacScale;
        cacScaleVector.z = 1.0f;
        refText.transform.localScale = cacScaleVector;

        moveOffset = moveCurve.Evaluate(curTime - startTime);
        cacMoveVector.y = moveOffset;
        refText.transform.localPosition = cacMoveVector;

        colorAlpha = alphaCurve.Evaluate(curTime - startTime);
        //textColor = refText.color;
        textColor.a = colorAlpha;
        refText.color = textColor;
    }

    public void DamageSpawn(int damage, Color color)
    {
        damageValue = damage;

        textColor = color;
    }
}
