using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneDirector : BaseSceneDirector
{
    // Start is called before the first frame update
    // protected override void Start()
    // {
        
    // }

    // Update is called once per frame
    protected override void Update()
    {
        Vector3 cam_pos = m_camera.transform.position;
        fade_sprite.transform.position = new Vector3(cam_pos.x, cam_pos.y, fade_sprite.transform.position.z);
    }
}
