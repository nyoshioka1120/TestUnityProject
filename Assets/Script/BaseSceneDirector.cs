using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneDirector : MonoBehaviour
{
    protected float FADE_SPEED = 2.0f;
    [SerializeField] protected string next_scene = "";
    [SerializeField] protected GameObject fade_sprite;
    protected SpriteRenderer sprite_renderer = new SpriteRenderer();
    protected GameObject m_camera;
    bool is_transition = false;
    bool is_fadeout = false;
    float fade_alpha = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_camera = GameObject.Find("Main Camera");
        sprite_renderer = fade_sprite.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && is_fadeout == false)
        {
            is_fadeout = true;
        }

        Vector3 cam_pos = m_camera.transform.position;
        fade_sprite.transform.position = new Vector3(cam_pos.x, cam_pos.y, fade_sprite.transform.position.z);

        if(FadeOut())
        {
            LoadScene();
        }
    }

    public bool FadeOut()
    {
        if(is_fadeout == false)
        {
            return false;
        }

        if(fade_alpha >= 255.0f)
        {
            return true;
        }

        fade_alpha += FADE_SPEED;
        sprite_renderer.color = new Color(0, 0, 0, fade_alpha / 255.0f);

        return false;
    }

    public void LoadScene()
    {
        if(next_scene == "") return;
        if(is_transition) return;

        is_transition = true;
        SceneManager.LoadScene(next_scene);
    }

    public void StartFadeOut()
    {
        is_fadeout = true;
    }
}
