using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDirector : MonoBehaviour
{
    [SerializeField] TitleSceneDirector m_scene_director;

    // Start is called before the first frame update
    void Start()
    {
        m_scene_director.StartFadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        m_scene_director.FadeIn();

        if(m_scene_director.IsFadeIn())
        {
            return;
        }

        if(m_scene_director.FadeOut())
        {
            m_scene_director.LoadScene();
        }

        if(m_scene_director.IsFadeOut())
        {
            return;
        }

        if(m_scene_director.IsTransition())
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            m_scene_director.StartFadeOut();
        }
    }
}
