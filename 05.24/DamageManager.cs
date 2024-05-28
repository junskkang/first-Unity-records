using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public Transform damageCanvas = null;
    public GameObject damagePrefab = null;

    Vector3 startPos = Vector3.zero;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageText(int Value, Vector3 ownerPos, Color ownerColor)
    {
        GameObject damageClone = Instantiate(damagePrefab);
        if (damageClone != null && damageCanvas != null) 
        {
            startPos = new Vector3(ownerPos.x, ownerPos.y + 2.0f, ownerPos.z);
            damageClone.transform.SetParent(damageCanvas);
            DamageCtrl damageText = damageClone.GetComponent<DamageCtrl>();
            damageText.DamageSpawn(Value, ownerColor);
            damageClone.transform.position = startPos;
        }

    }
}
