using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MIN_SPEED = 0.25f;
    const float MAX_WALK_SPEED = 3.0f;
    const float MAX_RUN_SPEED = 6.0f;
    const float SPEED_UP_RATE = 0.9f;
    const float SPEED_DOWN_RATE = 0.9f;

    const float JUMP_POWER = 10.0f;
    const float ADD_JUMP_POWER = 1.0f;
    const int ON_JUMP_FRAME = 8;

    const float SHOOT_ANIMATION_TIME = 1.0f / 2.0f;

    enum JUMP_STATE
    {
        NONE,
        RISING,
        FALLING,
        LANDING
    }

    SpriteRenderer m_sprite_renderer;
    Animator m_animator;

    Rigidbody2D m_rigidbody;
    BoxCollider2D m_box_collider;

    Vector2 m_scale = new Vector2(0.5f, 0.5f);

    int m_dir = 1;
    float m_speed = 0;
    JUMP_STATE m_jump_state = 0;
    int m_jump_frame = 0;
    float m_ray_distance = 0.1f;
    Vector3 m_ray_offset = new Vector3(0.0f, 0.0f, 0.0f);
    bool m_is_ground = false;

    public GameObject m_bullet_prefab;

    string m_now_anime_state = "Idle";
    string m_pre_anime_state = "Idle";

    float m_shoot_anime_time = -1.0f;
    [SerializeField] private float normalized_time = 0;

    Vector2 m_pre_velocity = new Vector2(0, 0);

    [SerializeField] private LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_box_collider = GetComponent<BoxCollider2D>();

        m_sprite_renderer = GetComponent<SpriteRenderer>();
        m_ray_distance = m_sprite_renderer.bounds.size.y * 0.5f + 0.05f;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Jump();

        Shoot();

        Animation();

        DebugKey();
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
        if(Input.GetKeyUp(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            dir = -1;
        }
        if(Input.GetKeyUp(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow))
        {
            dir = 1;
        }
        if(Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
        {
            dir = 0;
            ChangeAnimetion("Idle");
            return;
        }

        // 方向転換
        if(m_dir != dir && dir != 0)
        {
            m_dir = dir;
            m_speed = 0;
            transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.x, 1);
        }

        float max_speed = Input.GetKey(KeyCode.Z) ? MAX_RUN_SPEED : MAX_WALK_SPEED;

        if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
        {
            m_speed += SPEED_UP_RATE;
            if(m_speed > max_speed)
            {
                m_speed = max_speed;
            }
        }
        else
        {
            m_speed -= SPEED_UP_RATE;
            if(m_speed < 0)
            {
                m_speed = 0;
            }
        }

        if(!IsJumping() && !IsRunning())
        {
            if(m_speed == 0)
            {
                ChangeAnimetion("Idle");
            }
            else if(m_speed > 0 && m_speed < MAX_RUN_SPEED)
            {
                ChangeAnimetion("Walk");
            }
            else if(m_speed >= MAX_RUN_SPEED)
            {
                ChangeAnimetion("Run");
            }
        }

        m_rigidbody.velocity = new Vector2(m_speed * m_dir, m_rigidbody.velocity.y);
        
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
        m_ray_offset = new Vector3(m_box_collider.offset.x * 0.5f * m_dir, 0.0f, 0.0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position + m_ray_offset, Vector2.down, m_ray_distance, groundLayer);
        m_is_ground = hit.collider != null;

        if(!m_is_ground && m_rigidbody.velocity.y < 0)
        {
            m_jump_state = JUMP_STATE.FALLING;
        }

        if(m_is_ground && m_jump_state == JUMP_STATE.FALLING)
        {
            m_jump_state = JUMP_STATE.NONE;
            m_jump_frame = 0;
            OnJumpAnimeEnd();
        }

        if(Input.GetKeyDown(KeyCode.X) && m_is_ground)
        {
            m_jump_state = JUMP_STATE.RISING;
            m_jump_frame = 0;
            m_rigidbody.AddForce(transform.up * JUMP_POWER, ForceMode2D.Impulse);
            ChangeAnimetion("JumpStart");
        }

        if(Input.GetKey(KeyCode.X) && m_jump_state == JUMP_STATE.RISING &&  m_jump_frame < ON_JUMP_FRAME)
        {
            m_jump_frame++;
            m_rigidbody.AddForce(transform.up * ADD_JUMP_POWER, ForceMode2D.Impulse);
        }
    }

    void Shoot()
    {
        normalized_time = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if(Input.GetKeyDown(KeyCode.C))
        {
            GameObject bullet = Instantiate(m_bullet_prefab) as GameObject;
            Vector3 vec = new Vector3(transform.position.x + (1.0f * m_dir), transform.position.y, 0);
            bullet.GetComponent<BulletController>().Shoot(vec, m_dir);

            normalized_time = normalized_time % 1;

            if(IsJumping())
            {
                normalized_time = 0;
                ChangeAnimetion("JumpingShoot", 0);
            }
            else if(m_speed == 0)
            {
                normalized_time = 0;
                ChangeAnimetion("Shoot", 0);
            }
            else if(m_speed > 0 && m_speed < MAX_RUN_SPEED)
            {
                ChangeAnimetion("WalkShoot", normalized_time);
            }
            else if(m_speed >= MAX_RUN_SPEED)
            {
                ChangeAnimetion("RunShoot", normalized_time);
            }

            m_shoot_anime_time = normalized_time;
        }

        if(normalized_time - m_shoot_anime_time > SHOOT_ANIMATION_TIME && m_shoot_anime_time >= 0)
        {
            OnShootAnimeEnd();
        }
    }

    void Animation()
    {
    }

    bool ChangeAnimetion(string _anime, float _fixed_time = -1.0f)
    {
        if(m_now_anime_state == _anime && _fixed_time == -1.0f)
        {
            return false;
        }

        m_pre_anime_state = m_now_anime_state;
        m_now_anime_state = _anime;

        if(_fixed_time == -1.0f)
        {
            m_animator.SetTrigger(_anime);
        }
        else
        {
            m_animator.Play(_anime, -1, _fixed_time);
        }

        return true;
    }

    bool IsJumping()
    {
        return m_jump_frame > 0;
    }

    void OnJumpAnimeEnd()
    {
        if(m_speed == 0)
        {
            ChangeAnimetion("Idle");
        }
        else if(m_speed > 0 && m_speed < MAX_RUN_SPEED)
        {
            ChangeAnimetion("Walk");
        }
        else if(m_speed >= MAX_RUN_SPEED)
        {
            ChangeAnimetion("Run");
        }
    }

    bool IsRunning()
    {
        return m_shoot_anime_time != -1.0f;
    }

    void OnShootAnimeEnd()
    {
        if(IsJumping())
        {
            ChangeAnimetion("JumpStart", 0.1f);
        }
        else if(m_speed == 0)
        {
            ChangeAnimetion("Idle");
        }
        else if(m_speed > 0 && m_speed < MAX_RUN_SPEED)
        {
            ChangeAnimetion("Walk");
        }
        else if(m_speed >= MAX_RUN_SPEED)
        {
            ChangeAnimetion("Run");
        }

        m_shoot_anime_time = -1.0f;
    }

    void DebugKey()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            m_jump_state = JUMP_STATE.RISING;
            m_animator.speed = 1;
            m_animator.SetTrigger("Idle");
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = m_is_ground ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position + m_ray_offset, new Vector3(0, -m_ray_distance, 0));
    }
}
