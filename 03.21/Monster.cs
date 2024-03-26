using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyTeam 
{ 
    public class Monster
    {
        public string m_Name;
        public int m_Hp;
        public int m_Mp;
        public int m_Attack;
   
        public void PrintInfo()
        {
            Debug.Log($"이름 : {m_Name}, 체력({m_Hp}), 마나({m_Mp}), 공격력({m_Attack})");

        }
    }
}