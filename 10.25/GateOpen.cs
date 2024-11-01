using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    MonsterGenerator monsterGenerator;
    // Start is called before the first frame update
    void Start()
    {
        monsterGenerator = GetComponentInParent<MonsterGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DoorOpen()
    {
        monsterGenerator.CoOpenDoor();
    }
}
