using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMite : EnemyBase
{
    Animator m_animator;
    BoxCollider2D m_box_collider;

    [SerializeField] private LayerMask groundLayer;
    float m_ray_distance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();

        m_speed = 1.0f;
        m_dir = Random.Range(0,2) * 2 -1;
        m_scale = new Vector2(0.5f, 0.5f);
        transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.y, 1);

        m_animator = GetComponent<Animator>();
        m_animator.SetTrigger("Walk");

        m_ray_distance = GetComponent<SpriteRenderer>().bounds.size.x * m_scale.x + 0.05f;

        m_hp = 5;

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(1.0f * m_dir, 0), m_ray_distance, groundLayer);

        if(hit.collider != null)
        {
            m_dir = m_dir * -1;
            transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.x, 1);
        }

        m_rigidbody.velocity = new Vector2(m_speed * m_dir, m_rigidbody.velocity.y);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            Damage(1);
        }
    }

    protected override void Damage(int _damage)
    {
        Debug.Log("EnemyMite::Damage");
        m_rigidbody.velocity = new Vector2(0, 0);

        m_hp -= _damage;

        if(m_hp <= 0)
        {
            Dead();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(m_ray_distance * m_dir, 0, 0));
    }
}
