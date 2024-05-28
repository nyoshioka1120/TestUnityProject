using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBase : MonoBehaviour
{
    Rigidbody2D m_rigidbody;
    float m_impact = 7.0f;
    Vector3 m_dir = new Vector3(0f, 0f, 0f);
    float m_start_time = 0;
    const float LIMIT_TIME = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_start_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - m_start_time > LIMIT_TIME)
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 _start, Vector3 _target)
    {
        transform.position = _start;
        m_dir = _target - _start;
        m_dir = m_dir.normalized;

        m_rigidbody = GetComponent<Rigidbody2D>();
        m_rigidbody.AddForce(m_dir * m_impact, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 7)
        {
            Destroy(gameObject);
        }
    }

    public void Damaged()
    {
        Destroy(gameObject);
    }
}
