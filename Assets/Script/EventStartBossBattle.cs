using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventStartBossBattle : OnTouchTrigger
{
    GameObject m_target;

    // // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        var all_obj = FindObjectsOfType<GameObject>(true);

        foreach (var obj in all_obj)
        {
            if(obj.name == "EnemyGunnerPrefab(Clone)")
            {
                m_target = obj;
                m_target.SetActive(false);
                break;
            }
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public override void Event()
    {
        if(m_target == null) return;

        m_target.SetActive(true);
    }
}
