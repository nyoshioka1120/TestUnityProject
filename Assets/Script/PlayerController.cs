using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MIN_SPEED = 0.25f;
    const float MAX_WALK_SPEED = 3.0f;
    const float MAX_DASH_SPEED = 6.0f;
    const float SPEED = 50.0f;
    const float SPEED_UP_RATE = 0.9f;
    const float SPEED_DOWN_RATE = 0.9f;

    const float JUMP_POWER = 10.0f;
    const float ADD_JUMP_POWER = 1.0f;
    const int ON_JUMP_FRAME = 8;

    enum JUMP_STATE
    {
        NONE,
        RISING,
        FALLING
    }

    Animator m_animator;
    Rigidbody2D m_rigidbody;
    Vector2 m_scale = new Vector2(0.5f, 0.5f);
    int m_dir = 1;
    float m_speed = 0;
    JUMP_STATE m_jump_state = 0;
    int m_jump_frame = 0;
    float m_anime_frame = 0;

    public GameObject m_bullet_prefab;

    [SerializeField] Vector2 m_pre_velocity = new Vector2(0, 0);
    [SerializeField] float m_time = 0;
    [SerializeField] string m_clip_name = "";

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Jump();

        Shoot();

        Animation();

        Debug();
    }

    void Move()
    {
        m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x * SPEED_DOWN_RATE, m_rigidbody.velocity.y);

        int dir = m_dir;

        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            dir = 1;
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            dir = -1;
        }

        if(m_dir != dir)
        {
            m_dir = dir;
            transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.x, 1);
        }

        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_speed += SPEED_UP_RATE;
            m_rigidbody.velocity = new Vector2(m_speed * m_dir, m_rigidbody.velocity.y);
        }

        float max_speed = Input.GetKey(KeyCode.Z) ? MAX_DASH_SPEED : MAX_WALK_SPEED;
        
        if(Mathf.Abs(m_rigidbody.velocity.x) > max_speed)
        {
            m_rigidbody.velocity = new Vector2(max_speed * m_dir, m_rigidbody.velocity.y);
        }

        if(Mathf.Abs(m_rigidbody.velocity.x) < MIN_SPEED)
        {
            m_rigidbody.velocity = new Vector2(0, m_rigidbody.velocity.y);
        }

        m_pre_velocity = m_rigidbody.velocity;
    }

    void Jump()
    {
        if(Input.GetKeyDown(KeyCode.X) && m_rigidbody.velocity.y == 0)
        {
            m_jump_state = JUMP_STATE.RISING;
            m_jump_frame = 0;
            m_rigidbody.AddForce(transform.up * JUMP_POWER, ForceMode2D.Impulse);

            m_animator.SetTrigger("Jump");


            return;
        }

        if(Input.GetKey(KeyCode.X) && m_jump_state == JUMP_STATE.RISING &&  m_jump_frame < ON_JUMP_FRAME)
        {
            m_jump_frame++;
            m_rigidbody.AddForce(transform.up * ADD_JUMP_POWER, ForceMode2D.Impulse);
        }

        if((m_jump_state == JUMP_STATE.RISING && Input.GetKeyUp(KeyCode.X)) || (m_jump_state == JUMP_STATE.RISING &&  m_jump_frame >= ON_JUMP_FRAME))
        {
            m_jump_state = JUMP_STATE.FALLING;
        }

        if(Input.GetKeyDown(KeyCode.X) && m_rigidbody.velocity.y == 0)
        {
            m_jump_state = JUMP_STATE.NONE;
            m_jump_frame = 0;
            m_animator.speed = 1.0f;
        }
    }

    void Shoot()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            GameObject bullet = Instantiate(m_bullet_prefab) as GameObject;
            Vector3 vec = new Vector3(transform.position.x + (1.0f * m_dir), transform.position.y, 0);
            bullet.GetComponent<BulletController>().Shoot(vec, m_dir);
        }
    }

    void Animation()
    {
        float speed = Mathf.Abs(m_rigidbody.velocity.x);
        if(speed == 0)
        {
            // 待機
            m_animator.SetTrigger("Idle");
        }
        if(speed > 0 && speed <= MAX_WALK_SPEED)
        {
            // 歩き
            m_animator.SetTrigger("Walk");
        }
        if(speed > MAX_WALK_SPEED)
        {
            // 走り
            m_animator.SetTrigger("Run");
        }

        AnimatorStateInfo state_info = m_animator.GetCurrentAnimatorStateInfo(0);
        m_time = state_info.normalizedTime;
        m_clip_name = m_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if(m_clip_name == "Jump" && m_jump_state == JUMP_STATE.RISING)
        {
            if(state_info.normalizedTime >= 0.3f)
            {
                m_animator.speed = 0;

            }
        }
        if(m_clip_name == "Jump" && m_jump_state == JUMP_STATE.NONE)
        {
            m_animator.SetTrigger("Idle");
        }
    }

    void Debug()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            m_animator.speed = 1;
            m_animator.SetTrigger("Idle");
        }
    }
}
