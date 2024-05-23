using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventController : MonoBehaviour
{
    [SerializeField] GameObject trigger_prefab;

    List<GameObject> m_list = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate(string _name, string _event, Vector3 _pos)
    {
        GameObject trigger = Instantiate(trigger_prefab) as GameObject;
        trigger.AddComponent(Type.GetType(_event));
        trigger.GetComponent<OnTouchTrigger>().InitTrigger(_name, _pos);

        AddTrigger(trigger);
    }

    public void AddTrigger(GameObject _obj)
    {
        m_list.Add(_obj);
    }

    public void EventStart(string _name)
    {
        foreach(GameObject trigger in m_list)
        {
            string name = trigger.GetComponent<OnTouchTrigger>().GetName();
            if(name == _name)
            {
                Destroy(trigger);
            }
        }
    }

    public void InitTriggers()
    {
        foreach(GameObject trigger in m_list)
        {
            trigger.GetComponent<OnTouchTrigger>().Start();
        }
    }
}
