using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolNodeCtrl : MonoBehaviour
{
    [HideInInspector] public SkillType skillType;
    float skillTime = 0.0f;
    float skillDelay = 0.0f;
    public Image timeImg = null;
    public Image iconImg = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        skillTime -= Time.deltaTime;
        timeImg.fillAmount = skillTime / skillDelay;

        if (skillTime <= 0.0f)
            Destroy(gameObject);
    }

    public void InitState(float time, float delay)
    {
        skillTime = time;
        skillDelay = delay;
    }
}
