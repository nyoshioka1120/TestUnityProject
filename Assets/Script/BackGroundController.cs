using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundController : MonoBehaviour
{
    [SerializeField] GameObject m_bg_prefab;

    GameObject m_camera;
    Vector3 m_camera_pos = new Vector3(0, 0, 0);

    GameObject m_bg_filter_1;

    GameObject m_bg_1;
    GameObject m_bg_2;
    GameObject m_bg_3;

    const float BG_W = 18.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GameObject.Find("Main Camera");
        m_camera_pos = m_camera.transform.position;

        m_bg_filter_1 = GameObject.Find("BackGroundFilter1");
        m_bg_filter_1.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 1.0f);

        m_bg_1 = Instantiate(m_bg_prefab);
        m_bg_1.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 2.0f);

        m_bg_2 = Instantiate(m_bg_prefab);
        m_bg_2.transform.position = new Vector3(m_camera_pos.x + BG_W, m_camera_pos.y, 2.0f);

        m_bg_3 = Instantiate(m_bg_prefab);
        m_bg_3.transform.position = new Vector3(m_camera_pos.x - BG_W, m_camera_pos.y, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        m_camera_pos = m_camera.transform.position;
        m_bg_filter_1.transform.position = new Vector3(m_camera_pos.x, m_camera_pos.y, 1.0f);

        Vector3 bg_pos_1 = new Vector3(m_camera_pos.x / BG_W * BG_W + BG_W, m_camera_pos.y, 2.0f);
        Vector3 bg_pos_2 = new Vector3(m_camera_pos.x / BG_W * BG_W - BG_W, m_camera_pos.y, 2.0f);

        float length = 0;
        length = Mathf.Abs(m_camera_pos.x - m_bg_1.transform.position.x);
        if(length >= BG_W * 2)
        {
            m_bg_1.transform.position = m_camera_pos.x > m_bg_1.transform.position.x ? bg_pos_1 : bg_pos_2;
        }

        length = Mathf.Abs(m_camera_pos.x - m_bg_2.transform.position.x);
        if(length >= BG_W * 2)
        {
            m_bg_2.transform.position = m_camera_pos.x > m_bg_2.transform.position.x ? bg_pos_1 : bg_pos_2;
        }

        length = Mathf.Abs(m_camera_pos.x - m_bg_3.transform.position.x);
        if(length >= BG_W * 2)
        {
            m_bg_3.transform.position = m_camera_pos.x > m_bg_3.transform.position.x ? bg_pos_1 : bg_pos_2;
        }
    }
}
