using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int m_hp = 1;

    protected int m_dir = 1;
    protected float m_speed = 0;

    protected Rigidbody2D m_rigidbody;

    protected Vector2 m_scale = new Vector2(1.0f, 1.0f);

    protected string m_unique_id = "";
    

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    void SetHp(int _hp)
    {
        m_hp = _hp;
    }

    protected virtual void Damage(int _damage)
    {
        Debug.Log("EnemyBase::Damage");
    }

    public bool IsLive()
    {
        return m_hp > 0;
    }

    protected virtual void Dead()
    {
        GameObject director = GameObject.Find("GameDirector");
        if(director)
        {
            director.GetComponent<GameDirector>().SetCleared(GetUID());
        }

        Destroy(gameObject);
    }

    public string GetUID()
    {
        return m_unique_id;
    }

    public void SetUID()
    {
        if(m_unique_id != "") return;
        m_unique_id = Guid.NewGuid().ToString("N");
    }
}
