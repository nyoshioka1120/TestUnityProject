using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BaseSceneDirector : MonoBehaviour
{
    [SerializeField] protected float FADE_SPEED = 2.0f;
    [SerializeField] protected string next_scene = "";
    [SerializeField] protected GameObject canvas_prefab;
    protected GameObject m_canvas;
    protected Image m_fade;
    protected SpriteRenderer sprite_renderer = new SpriteRenderer();
    bool is_transition = false;
    bool is_fadein = false;
    bool is_fadeout = false;
    float fade_alpha = 0;

    protected virtual void Awake()
    {
        m_canvas = Instantiate(canvas_prefab);
        m_fade = m_canvas.transform.Find("Fade").gameObject.GetComponent<Image>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && is_fadeout == false)
        {
            is_fadeout = true;
        }
    }

    public bool FadeIn()
    {
        if(is_fadein == false)
        {
            return false;
        }

        if(fade_alpha <= 0)
        {
            is_fadein = false;
            return true;
        }

        fade_alpha -= FADE_SPEED;
        if(fade_alpha < 0) fade_alpha = 0;

        m_fade.color = new Color (0, 0, 0, fade_alpha / 255.0f);

        return false;
    }

    public bool FadeOut()
    {
        if(is_fadeout == false)
        {
            return false;
        }

        if(fade_alpha >= 255.0f)
        {
            is_fadeout = false;
            return true;
        }

        fade_alpha += FADE_SPEED;
        if(fade_alpha > 255.0f) fade_alpha = 255.0f;

        m_fade.color = new Color (0, 0, 0, fade_alpha / 255.0f);

        return false;
    }

    public void StartFadeIn()
    {
        is_fadein = true;
        fade_alpha = 255.0f;
        m_fade.color = new Color (0, 0, 0, fade_alpha / 255.0f);
        Update();
    }

    public void StartFadeOut()
    {
        is_fadeout = true;
        fade_alpha = 0;
        m_fade.color = new Color (0, 0, 0, fade_alpha / 255.0f);
        Update();
    }

    public bool IsFadeIn()
    {
        return is_fadein;
    }

    public bool IsFadeOut()
    {
        return is_fadeout;
    }

    public void LoadScene()
    {
        if(next_scene == "") return;
        if(is_transition) return;

        is_transition = true;
        SceneManager.LoadScene(next_scene);
    }

    public bool IsTransition()
    {
        return is_transition;
    }
}
