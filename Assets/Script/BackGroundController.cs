using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField] GameObject m_bg_far_prefab;
    [SerializeField] GameObject m_bg_near_prefab;

    GameObject m_camera;
    Vector3 m_camera_pos = new Vector3(0, 0, 0);
    Vector3 m_camera_pos_pre = new Vector3(0, 0, 0);

    // GameObject m_bg_filter_far;
    // GameObject m_bg_filter_near;

    GameObject m_bg_1;
    GameObject m_bg_2;
    GameObject m_bg_3;

    List<GameObject> m_bg_far;
    List<GameObject> m_bg_near;

    const float BG_W = 18.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GameObject.Find("Main Camera");
        m_camera_pos = m_camera.transform.position;
        m_camera_pos_pre = m_camera_pos;

        // m_bg_filter_far = GameObject.Find("BackGroundFilterFar");
        // m_bg_filter_far.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 3.0f);

        m_bg_far = new List<GameObject>();

        var bg_far_1 = Instantiate(m_bg_far_prefab);
        bg_far_1.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 4.0f);
        m_bg_far.Add(bg_far_1);

        var bg_far_2 = Instantiate(m_bg_far_prefab);
        bg_far_2.transform.position = new Vector3(m_camera_pos.x + BG_W, m_camera_pos.y, 4.0f);
        m_bg_far.Add(bg_far_2);

        var bg_far_3 = Instantiate(m_bg_far_prefab);
        bg_far_3.transform.position = new Vector3(m_camera_pos.x - BG_W, m_camera_pos.y, 4.0f);
        m_bg_far.Add(bg_far_3);


        // m_bg_filter_near = GameObject.Find("BackGroundFilterNear");
        // m_bg_filter_near.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 1.0f);

        m_bg_near = new List<GameObject>();

        var bg_near_1 = Instantiate(m_bg_near_prefab);
        bg_near_1.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 2.0f);
        m_bg_near.Add(bg_near_1);

        var bg_near_2 = Instantiate(m_bg_near_prefab);
        bg_near_2.transform.position = new Vector3(m_camera_pos.x + BG_W, m_camera_pos.y, 2.0f);
        m_bg_near.Add(bg_near_2);

        var bg_near_3 = Instantiate(m_bg_near_prefab);
        bg_near_3.transform.position = new Vector3(m_camera_pos.x - BG_W, m_camera_pos.y, 2.0f);
        m_bg_near.Add(bg_near_3);
    }

    // Update is called once per frame
    void Update()
    {
        m_camera_pos = m_camera.transform.position;

        // m_bg_filter_far.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 3.0f);
        // m_bg_filter_near.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 1.0f);

        Vector3 bg_pos_far_1 = new Vector3(m_camera_pos.x / BG_W * BG_W + BG_W, m_camera_pos.y, 4.0f);
        Vector3 bg_pos_far_2 = new Vector3(m_camera_pos.x / BG_W * BG_W - BG_W, m_camera_pos.y, 4.0f);

        float length = 0;

        foreach(var bg in m_bg_far)
        {
            Vector3 add = (m_camera_pos - m_camera_pos_pre) * 0.5f;
            bg.transform.position += add;

            length = Mathf.Abs(m_camera_pos.x - bg.transform.position.x);
            if(length >= BG_W * 2)
            {
                bg.transform.position = m_camera_pos.x > bg.transform.position.x ? bg_pos_far_1 : bg_pos_far_2;
            }
        }

        Vector3 bg_pos_near_1 = new Vector3(m_camera_pos.x / BG_W * BG_W + BG_W, m_camera_pos.y, 2.0f);
        Vector3 bg_pos_near_2 = new Vector3(m_camera_pos.x / BG_W * BG_W - BG_W, m_camera_pos.y, 2.0f);

        foreach(var bg in m_bg_near)
        {
            length = Mathf.Abs(m_camera_pos.x - bg.transform.position.x);
            if(length >= BG_W * 2)
            {
                bg.transform.position = m_camera_pos.x > bg.transform.position.x ? bg_pos_near_1 : bg_pos_near_2;
            }
        }

        m_camera_pos_pre = m_camera_pos;
    }
}
