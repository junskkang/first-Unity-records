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
            Debug.Log($"�̸� : {m_Name}, ü��({m_Hp}), ����({m_Mp}), ���ݷ�({m_Attack})");

        }
    }
}