using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class MovieController : MonoBehaviour
{
    enum MOVIE_STATE
    {
        NONE,
        LB_IN,
        SCENARIO,
        LB_OUT
    }
    [SerializeField] Canvas m_canvas;
    [SerializeField] Image m_lb_top;
    [SerializeField] Image m_lb_bottom;
    [SerializeField] Text m_textbox;

    Vector3 LB_TOP_START_POS {get{return new Vector3(0, 474.5f + 131.0f, 0);}}
    Vector3 LB_TOP_END_POS {get{return new Vector3(0, 474.5f, 0);}}
    Vector3 LB_BOTTOM_START_POS {get{return new Vector3(0, -474.5f - 131.0f, 0);}}
    Vector3 LB_BOTTOM_END_POS {get{return new Vector3(0, -474.5f, 0);}}
    const float LB_MOVE_TIME = 0.5f;
    float m_start_time = 0f;

    ScenarioDatas m_scenario_data;
    string scenario_file_name = "01_01";
    string scenario_file_dir = "ScenarioData/";

    int text_length = 0;
    float text_start = 0;
    float text_interval = 0.1f;
    float next_text_interval = 1.0f;

    MOVIE_STATE m_movie_state = MOVIE_STATE.NONE;

    bool is_exit = false;

    // Start is called before the first frame update
    void Start()
    {
        m_lb_top.transform.localPosition = LB_TOP_START_POS;
        m_lb_bottom.transform.localPosition = LB_BOTTOM_START_POS;
        m_textbox.text = "";

        LoadScenario();

        MovieStart();
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_movie_state)
        {
            case MOVIE_STATE.NONE:
            {
                break;
            }
            case MOVIE_STATE.LB_IN:
            {
                LBEntry();
                break;
            }
            case MOVIE_STATE.SCENARIO:
            {
                ScenarioUpdate();
                break;
            }
            case MOVIE_STATE.LB_OUT:
            {
                LBExit();
                break;
            }
            default:
            {
                break;
            }
        }
    }

    void MovieStart()
    {
        m_start_time = Time.time;

        m_movie_state = MOVIE_STATE.LB_IN;
    }

    bool LBEntry()
    {
        if(Time.time - m_start_time > LB_MOVE_TIME)
        {
            m_start_time = 0f;
            m_movie_state = MOVIE_STATE.SCENARIO;
            return false;
        }

        m_lb_top.transform.localPosition = Vector3.Lerp(LB_TOP_START_POS, LB_TOP_END_POS, Mathf.SmoothStep(0f, 1f, (Time.time - m_start_time) / LB_MOVE_TIME)); 
        m_lb_bottom.transform.localPosition = Vector3.Lerp(LB_BOTTOM_START_POS, LB_BOTTOM_END_POS, Mathf.SmoothStep(0f, 1f, (Time.time - m_start_time) / LB_MOVE_TIME));
        
        return true;
    }

    bool LBExit()
    {
        if(Time.time - m_start_time > LB_MOVE_TIME)
        {
            m_start_time = 0f;
            is_exit = true;
            m_movie_state = MOVIE_STATE.NONE;
            return false;
        }

        m_lb_top.transform.localPosition = Vector3.Lerp(LB_TOP_END_POS, LB_TOP_START_POS, Mathf.SmoothStep(0f, 1f, (Time.time - m_start_time) / LB_MOVE_TIME)); 
        m_lb_bottom.transform.localPosition = Vector3.Lerp(LB_BOTTOM_END_POS, LB_BOTTOM_START_POS, Mathf.SmoothStep(0f, 1f, (Time.time - m_start_time) / LB_MOVE_TIME));
        
        return true;
    }

    void LoadScenario()
    {
        string json = Resources.Load(scenario_file_dir + scenario_file_name).ToString();
        m_scenario_data = JsonMapper.ToObject<ScenarioDatas>(json);
    }

    bool ScenarioUpdate()
    {
        if(m_scenario_data.data_list.Count == 0)
        {
            m_start_time = Time.time;
            m_movie_state = MOVIE_STATE.LB_OUT;
            return false;
        }

        foreach(ScenarioData data in m_scenario_data.data_list)
        {
            switch(data.type)
            {
                case "move":
                {
                    Move(data.data);
                    m_scenario_data.data_list.Remove(data);
                    break;
                }
                case "speak":
                {
                    if(Speak(data.data))
                    {
                        m_scenario_data.data_list.Remove(data);
                    }
                    break;
                }
                default:
                {
                    break;
                }
            }

            return true;
        }

        return true;
    }

    void Move(string _data)
    {
        string[] split_str = _data.Split(',');
        string name = "";
        Vector3 vec = new Vector3(0,0,0);
        string animation = "";
        
        foreach(string str in split_str)
        {
            string[] data = str.Split(':');
            switch(data[0])
            {
                case "name":
                {
                    name = data[1];
                    break;
                }
                case "x":
                {
                    vec.x = float.Parse(data[1]);
                    break;
                }
                case "y":
                {
                    vec.y = float.Parse(data[1]);
                    break;
                }
                case "z":
                {
                    vec.z = float.Parse(data[1]);
                    break;
                }
                case "animation":
                {
                    animation = data[1];
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        GameObject obj = GameObject.Find(name);
        if(obj == null)
        {
            return;
        }

        obj.GetComponent<Rigidbody2D>().velocity = new Vector2(vec.x, vec.y);
        obj.GetComponent<PlayerController>().ChangeAnimetion(animation);
    }

    bool Speak(string _data)
    {
        string[] split_str = _data.Split(',');
        string name = "";
        string text = "";
        
        foreach(string str in split_str)
        {
            string[] data = str.Split(':');
            switch(data[0])
            {
                case "name":
                {
                    name = data[1];
                    break;
                }
                case "text":
                {
                    text = data[1];
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        string draw_text = text.Substring(0, text_length);
        m_textbox.text = name + "：" + draw_text;

        if(Time.time - text_start > text_interval && text_length < text.Length)
        {
            text_start = Time.time;
            text_length++;
        }

        if(text_length >= text.Length)
        {
            if(Time.time - text_start > next_text_interval)
            {
                text_start = 0;
                text_length = 0;
                return true;
            }

            text_length = text.Length;
        }

        return false;
    }

    public bool IsExit()
    {
        return is_exit;
    }
}

class ScenarioDatas
{
    public List<ScenarioData> data_list = new List<ScenarioData>();
}

class ScenarioData
{
    public string type = "";
    public string data = "";
}