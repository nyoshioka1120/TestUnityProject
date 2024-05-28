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

    const float NO_DAMEGE_TIME = 1.0f;

    const int MAX_HP = 10;

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
    CircleCollider2D m_circle_collider;

    Vector2 m_scale = new Vector2(0.25f, 0.25f);

    int m_dir = 1;
    float m_speed = 0;
    JUMP_STATE m_jump_state = 0;
    int m_jump_frame = 0;
    float m_ray_distance = 0;
    Vector3 m_ray_offset = new Vector3(0.0f, 0.0f, 0.0f);
    bool m_is_ground = false;

    //public GameObject m_bullet_prefab;
    [SerializeField] private BulletController m_bullet_prefab;

    string m_now_anime_state = "Idle";
    string m_pre_anime_state = "Idle";

    float m_shoot_anime_time = -1.0f;

    Vector2 m_pre_velocity = new Vector2(0, 0);

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float radius = 0;

    bool m_control_enabled = true;

    int m_hp = MAX_HP;
    bool m_is_damaged = false;

    GameDirector m_game_director;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_box_collider = GetComponent<BoxCollider2D>();
        m_circle_collider = GetComponent<CircleCollider2D>();

        m_sprite_renderer = GetComponent<SpriteRenderer>();
        //m_ray_distance = m_sprite_renderer.bounds.size.y * 0.5f + 0.05f;
        m_ray_distance = (m_circle_collider.radius + 1.0f) * m_scale.x;
        radius = m_ray_distance;

        m_game_director = GameObject.Find("GameDirector").GetComponent<GameDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();

        if(m_control_enabled == false)
        {
            return;
        }

        if(m_is_damaged)
        {
            float normalized_time = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if(m_now_anime_state == "Damage" && normalized_time > NO_DAMEGE_TIME)
            {
                OnDamageAnimeEnd();
            }
            return;
        }

        if(IsDead())
        {
            return;
        }

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
            transform.localScale = new Vector3(m_dir * m_scale.x, m_scale.y, 1);
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
        //m_ray_offset = new Vector3(m_box_collider.offset.x * 0.5f * m_dir, 0.0f, 0.0f);
        // m_ray_offset = new Vector3(m_circle_collider.offset.x * m_scale.x * m_dir, m_circle_collider.offset.y * m_scale.y, 0.0f);

        // RaycastHit2D hit = Physics2D.Raycast(transform.position + m_ray_offset, Vector2.down, m_ray_distance, groundLayer);
        // m_is_ground = hit.collider != null;

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
        float normalized_time = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if(Input.GetKeyDown(KeyCode.C))
        {
            BulletController bullet = Instantiate(m_bullet_prefab) as BulletController;
            Vector3 pos = transform.Find("ShootPoint").position;
            bullet.Shoot(pos, m_dir);

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

    bool CheckGround()
    {
        m_ray_offset = new Vector3(m_circle_collider.offset.x * m_scale.x * m_dir, m_circle_collider.offset.y * m_scale.y, 0.0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position + m_ray_offset, Vector2.down, m_ray_distance, groundLayer);
        m_is_ground = hit.collider != null;

        return m_is_ground;
    }

    bool IsGround()
    {
        return m_is_ground;
    }

    void Animation()
    {
    }

    public bool ChangeAnimetion(string _anime, float _fixed_time = -1.0f)
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

    void OnDeadAnimeEnd()
    {
        m_game_director.PlayerDead();
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

    public void SetControlEnabled(bool _enabled)
    {
        //ChangeAnimetion("Idle");
        m_control_enabled = _enabled;

        if(_enabled == true)
        {
            m_rigidbody.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy" && other.gameObject.GetComponent<EnemyBase>().IsLive())
        {
            Damage(1);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "EnemyBullet")
        {
            Damage(1);
            other.gameObject.GetComponent<EnemyBulletBase>().Damaged();
        }
    }

    void Damage(int _damage)
    {
        if(m_is_damaged || IsDead())
        {
            return;
        }

        m_hp -= _damage;

        if(m_hp <= 0)
        {
            m_hp = 0;
        }

        m_is_damaged = true;

        Debug.Log("Player::Damage()");

        Vector2 force = new Vector2((-m_dir) * JUMP_POWER, JUMP_POWER);
        m_rigidbody.AddForce(force, ForceMode2D.Impulse);

        m_jump_state = JUMP_STATE.NONE;

        m_game_director.SetHPGauge(m_hp, MAX_HP);

        ChangeAnimetion("Damage", 0f);
    }

    void OnDamageAnimeEnd()
    {
        float normalized_time = m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if(m_is_ground && m_is_damaged && normalized_time > NO_DAMEGE_TIME)
        {
            m_is_damaged = false;

            if(IsDead())
            {
                Dead();
            }
            else
            {
                ChangeAnimetion("Idle");
            }
        }
    }

    void Dead()
    {
        m_rigidbody.bodyType = RigidbodyType2D.Kinematic;
        m_rigidbody.velocity = new Vector2(0f, 0f);
        ChangeAnimetion("Dead");
    }

    public bool IsDead()
    {
        return m_hp <= 0;
    }

    public int GetMaxHP()
    {
        return MAX_HP;
    }

    public int GetHP()
    {
        return m_hp;
    }

    public void SetPosition(Vector3 _pos)
    {
        Vector3 offset = new Vector3(0f,0.1f,0f);
        Vector3 pos = _pos + offset;
        transform.position = pos;

        transform.position = _pos;
    }

    void DebugKey()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Damage(1);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = m_is_ground ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position + m_ray_offset, new Vector3(0, -m_ray_distance, 0));
    }

    public void InitPlayerController()
    {
        Start();
    }
}
