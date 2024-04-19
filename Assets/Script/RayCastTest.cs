using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    Rigidbody2D m_rigidbody;

    bool m_is_ground = false;
    float m_ray_distance = 0.55f;

    [SerializeField] private LayerMask groundLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray2D ray = new Ray2D(transform.position, Vector2.down);
        //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, m_ray_distance);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, m_ray_distance, groundLayer);
        m_is_ground = hit.collider != null;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_rigidbody.AddForce(transform.up * 20.0f, ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(m_is_ground) Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, new Vector3(0, -m_ray_distance, 0));
    }
}
