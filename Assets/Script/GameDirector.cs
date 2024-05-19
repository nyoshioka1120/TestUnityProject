using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    enum GAME_MODE
    {
        NONE,
        GAME,
        MOVIE,
        CLEAR
    }

    GAME_MODE m_game_mode = GAME_MODE.NONE;

    PlayerController m_player_controller;
    [SerializeField] List<QuestData> m_clear_check_list = new List<QuestData>();

    [SerializeField] GameSceneDirector m_scene_director;

    // Start is called before the first frame update
    void Start()
    {
        LoadStageData();
        
        QuestData tmp_data = new QuestData();
        tmp_data.name = "tmp";
        m_clear_check_list.Add(tmp_data);

        m_game_mode = GAME_MODE.GAME;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_player_controller == null)
        {
            m_player_controller = GameObject.Find("Player").GetComponent<PlayerController>();
        }

        switch(m_game_mode)
        {
            case GAME_MODE.NONE:
            {
                break;
            }
            case GAME_MODE.GAME:
            {
                GameUpdate();
                break;
            }
            case GAME_MODE.MOVIE:
            {
                MovieUpdate();
                break;
            }
            case GAME_MODE.CLEAR:
            {
                ClearUpdate();
                break;
            }
            default:
            {
                break;
            }
        }
    }

    void LoadStageData()
    {

    }

    void MovieUpdate()
    {

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
            m_clear_check_list.Single(d => d.name == _name).is_cleared = true;
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
