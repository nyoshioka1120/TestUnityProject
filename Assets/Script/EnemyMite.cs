using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMite : EnemyBase
{
    Animator m_animator;
    BoxCollider2D m_box_collider;
    CircleCollider2D m_circle_collider;

    [SerializeField] private LayerMask groundLayer;
    float m_ray_distance = 0.1f;

    int m_action = 1;
    int m_action_cnt = 0;

    int IDLE_TIME = 60;
    int MOVE_TIME = 60;
    int SHOOT_TIME = 60;
    int SHOOT_INTERVAL = 60;

    bool is_target_range = false;

    [SerializeField] GameObject m_bullet_prefab;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        m_box_collider = GetComponent<BoxCollider2D>();
        m_circle_collider = GetComponent<CircleCollider2D>();

        m_speed = 1.0f;
        m_dir = Random.Range(0,2) * 2 -1;
        m_scale = new Vector2(0.5f, 0.5f);
        transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.y, 1);

        m_animator = GetComponent<Animator>();
        ChangeAction(2);
        
        m_ray_distance = m_box_collider.size.x * m_scale.x * 0.5f + 0.05f;

        m_hp = 5;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if(m_is_active == false) return;

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
                Move();
                break;
            }
            case 3:
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
                m_animator.SetTrigger("Walk");
                break;
            }
            case 3:
            {
                GameObject player = GameObject.Find("Player(Clone)");

                m_dir = player.transform.position.x > transform.position.x ? 1 : -1;
                transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.x, 1);

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
            int action = 1;

            if(is_target_range == true)
            {
                action = Random.Range(1, 4);
            }
            else
            {
                action = Random.Range(1, 3);
            }

            ChangeAction(action);
        }
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

        if(m_action_cnt > MOVE_TIME)
        {
            ChangeAction(1);
        }
    }

    void Shoot()
    {
        if(m_action_cnt % SHOOT_INTERVAL == 0)
        {
            var bullet = Instantiate(m_bullet_prefab);
            var shoot_point = transform.Find("ShootPoint");
            Vector3 target_pos = shoot_point.position + new Vector3(1.0f * m_dir, 0f, 0f);
            bullet.GetComponent<EnemyBulletBase>().Shoot(shoot_point.position, target_pos);
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

    void OnCollisionExit2D(Collision2D other)
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Main Camera")
        {
            SetActive(true);
        }

        if(other.gameObject.tag == "Player")
        {
            is_target_range = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.name == "Main Camera")
        {
            SetActive(false);
        }

        if(other.gameObject.tag == "Player")
        {
            is_target_range = false;
        }
    }

    protected override void Damage(int _damage)
    {
        Debug.Log("EnemyMite::Damage");
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, new Vector3(m_ray_distance * m_dir, 0, 0));
    }
}