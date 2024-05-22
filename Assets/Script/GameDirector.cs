using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    enum GAME_MODE
    {
        NONE,
        MOVIE,
        GAME,
        CLEAR
    }

    GAME_MODE m_game_mode = GAME_MODE.NONE;

    [SerializeField] GameObject m_player_prefab;
    GameObject m_player;
    PlayerController m_player_controller;
    List<QuestData> m_clear_check_list = new List<QuestData>();
    [SerializeField] int clear_list_size = 0;

    [SerializeField] GameSceneDirector m_scene_director;

    [SerializeField] MovieController m_movie_controller;

    // Start is called before the first frame update
    void Start()
    {
        LoadStageData();

        MovieStart();

        m_scene_director.StartFadeIn();

        m_game_mode = GAME_MODE.NONE;
    }

    // Update is called once per frame
    void Update()
    {
        clear_list_size = m_clear_check_list.Count;
        switch(m_game_mode)
        {
            case GAME_MODE.NONE:
            {
                break;
            }
            case GAME_MODE.MOVIE:
            {
                MovieUpdate();
                return;
            }
            case GAME_MODE.GAME:
            {
                GameUpdate();
                return;
            }
            case GAME_MODE.CLEAR:
            {
                ClearUpdate();
                return;
            }
            default:
            {
                return;
            }
        }

        if(m_scene_director.FadeIn())
        {
            m_game_mode = GAME_MODE.MOVIE;
        }

        if(m_scene_director.IsFadeIn())
        {
            return;
        }
    }

    void LoadStageData()
    {
        m_player = Instantiate(m_player_prefab) as GameObject;
        m_player_controller = m_player.GetComponent<PlayerController>();
        m_player_controller.InitPlayerController();

        var camera = GameObject.Find("Main Camera");
        camera.GetComponent<CameraController>().SetPlayer();
    }

    public void AddQuestData(string _name)
    {
        QuestData data = new QuestData();
        data.name = _name;
        m_clear_check_list.Add(data);

        //Debug.Log("Add::"+_name);
    }

    void MovieUpdate()
    {
        if(m_movie_controller.IsExit())
        {
            m_movie_controller.gameObject.SetActive(false);

            MovieEnd();
            m_game_mode = GAME_MODE.GAME;
        }
    }

    void GameUpdate()
    {
        if(ClearCheck() == true)
        {
            StageClear();
        }

        if(Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.C))
        {
            DebugStageClear();
        }
    }

    void ClearUpdate()
    {
        if(m_scene_director.FadeOut())
        {
            m_scene_director.LoadScene();
        }
    }

    void MovieStart()
    {
        m_player_controller.SetControlEnabled(false);
    }

    void MovieEnd()
    {
       m_player_controller.SetControlEnabled(true);
    }

    public void SetCleared(string _name = "")
    {
        if(_name != "")
        {
            QuestData data = m_clear_check_list.Single(d => d.name == _name);
            if(data != null)
            {
                data.is_cleared = true;
            }

            Debug.Log("Add::"+_name);
        }
    }

    bool ClearCheck()
    {
        foreach(var data in m_clear_check_list)
        {
            if(data.is_cleared == false)
            {
                return false;
            }
        }

        return true;
    }

    void StageClear()
    {
        m_game_mode = GAME_MODE.CLEAR;

        m_scene_director.StartFadeOut();

        MovieStart();
    }

    void DebugStageClear()
    {
        foreach(var data in m_clear_check_list)
        {
            data.is_cleared = true;
        }
    }

    void DebugCreateStageData()
    {

    }
}

public class QuestData
{
    public string name = "";
    public bool is_cleared = false;
}
