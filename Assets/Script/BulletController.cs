using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    float m_speed = 30.0f;
    int m_dir = 1;
    Rigidbody2D m_rigidbody;
    public GameObject m_particle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot(Vector3 _pos, int _dir)
    {
        transform.position = _pos;
        m_dir = _dir;
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_rigidbody.velocity = new Vector2(m_speed * m_dir, m_rigidbody.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        //GetComponent<ParticleSystem>().Play();

        GameObject particle = Instantiate(m_particle) as GameObject;
        particle.transform.position = transform.position;

        Destroy(gameObject);
    }
}
