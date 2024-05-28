using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGunner : EnemyBase
{
    Animator m_animator;
    BoxCollider2D m_box_collider;
    CircleCollider2D m_circle_collider;

    [SerializeField] private LayerMask groundLayer;
    float m_ray_distance = 0.1f;

    int m_action = 1;
    int m_action_cnt = 0;

    int IDLE_TIME = 60;
    int SHOOT_TIME = 60;
    int SHOOT_INTERVAL = 20;

    [SerializeField] GameObject m_bullet_prefab;

    Vector3 m_target_pos = new Vector3(0f,0f,0f);

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        m_box_collider = GetComponent<BoxCollider2D>();
        m_circle_collider = GetComponent<CircleCollider2D>();

        m_speed = 1.0f;
        m_dir = -1;
        m_scale = new Vector2(1.0f, 1.0f);
        transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.y, 1);

        m_animator = GetComponent<Animator>();
        ChangeAction(1);
        
        m_ray_distance = m_box_collider.size.x * m_scale.x * 0.5f + 0.05f;

        m_hp = 10;
    }

    // Update is called once per frame
    protected override void Update()
    {
        switch(m_action)
        {
            case 0:
            {
                //Dead();
                break;
            }
            case 1:
            {
                Idle();
                break;
            }
            case 2:
            {
                Shoot();
                break;
            }
        }

        m_action_cnt++;
    }

    void ChangeAction(int _action)
    {
        m_action_cnt = 0;

        if(m_action == _action)
        {
            return;
        }

        m_action = _action;

        switch(m_action)
        {
            case 0:
            {
                m_rigidbody.simulated = false;
                m_animator.SetTrigger("Dead");
                break;
            }
            case 1:
            {
                m_animator.SetTrigger("Idle");
                break;
            }
            case 2:
            {
                m_target_pos = GameObject.Find("Player(Clone)").transform.position;
                m_animator.SetTrigger("Idle");
                break;
            }
            default:
            {
                m_animator.SetTrigger("Idle");
                break;
            }
        }
    }

    void Idle()
    {
        if(m_action_cnt > IDLE_TIME)
        {
            ChangeAction(Random.Range(1, 3));
        }
    }

    void Shoot()
    {
        if(m_action_cnt % SHOOT_INTERVAL == 0)
        {
            var bullet = Instantiate(m_bullet_prefab);
            var shoot_point = transform.Find("ShootPoint");
            bullet.GetComponent<EnemyBulletBase>().Shoot(shoot_point.position, m_target_pos);
        }

        if(m_action_cnt > SHOOT_TIME)
        {
            ChangeAction(1);
        }
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
        Debug.Log("EnemyGunner::Damage");
        m_rigidbody.velocity = new Vector2(0, 0);

        m_hp -= _damage;

        if(m_hp <= 0)
        {
            ChangeAction(0);
        }
    }

    void OnDeadAnimeEnd()
    {
        Dead();
    }
}
