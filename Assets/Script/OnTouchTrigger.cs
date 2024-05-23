using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchTrigger : MonoBehaviour
{
    string m_name = "";

    // Start is called before the first frame update
    public virtual void Start()
    {
        SpriteRenderer sprite_renderer = GetComponent<SpriteRenderer>();
        sprite_renderer.color = new Color(0f, 0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("OnTriggerEnter2D::"+m_name);

            Event();

            GameObject controller = GameObject.Find("TriggerEventController");
            controller.GetComponent<TriggerEventController>().EventStart(m_name);
        }
    }

    public string GetName()
    {
        return m_name;
    }

    public void SetName(string _name)
    {
        m_name = _name;
    }

    public void InitTrigger(string _name, Vector3 _pos)
    {
        Start();

        m_name = _name;
        transform.position = _pos;
    }

    public virtual void Event()
    {
        Debug.Log("OnTouchTrigger::Event()");
    }
}
