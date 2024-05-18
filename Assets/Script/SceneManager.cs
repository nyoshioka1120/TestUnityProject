using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseSceneManager : MonoBehaviour
{
    [SerializeField] string next_scene = "";
    [SerializeField] SpriteRenderer fade_sprite;
    bool is_transition = false;
    bool is_fadein = false;
    float fade_alpha = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && is_fadein == false)
        {
            is_fadein = true;
        }

        if(FadeIn())
        {
            LoadScene();
        }
    }

    bool FadeIn()
    {
        if(is_fadein == false)
        {
            return false;
        }

        if(fade_alpha >= 255.0f)
        {
            return true;
        }

        fade_alpha++;
        fade_sprite.color = new Color(0, 0, 0, fade_alpha / 255.0f);

        return false;
    }

    void LoadScene()
    {
        if(next_scene == "") return;
        if(is_transition) return;

        is_transition = true;
        SceneManager.LoadScene(next_scene);
    }
}
