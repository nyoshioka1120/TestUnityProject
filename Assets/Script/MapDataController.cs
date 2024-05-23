using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using LitJson;

public class MapDataController : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] TileDataList tile_data_list;

    [SerializeField] GameObject empty_grid_prefab;
    [SerializeField] EnemyGenerator enemy_generator;
    [SerializeField] TriggerEventController trigger_controller;

    [SerializeField] string FILE_NAME = "map_data";
    [SerializeField] string FILE_DIR = "MapData/";

    // Start is called before the first frame update
    void Start()
    {
        Save();
        Load();
    }

    public void Save()
    {
        if(tilemap == null)
        {
            Debug.Log("[warning]tilemap not exist");
            return;
        }

        tilemap.CompressBounds();
        var cell_bounds = tilemap.cellBounds;

        MapDatas list = new MapDatas();
        list.setting.origin = tilemap.origin;
        list.setting.size = tilemap.size;

        for(int y = cell_bounds.min.y; y < cell_bounds.max.y; y++)
        {
            List<MapData> tile_datas = new List<MapData>();

            for(int x = cell_bounds.min.x; x < cell_bounds.max.x; x++)
            {
                MapData tile_data = new MapData();

                if(tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                    string name = tile_data_list.list.Single(t => t.tile == tile).name;
                    string type = tile_data_list.list.Single(t => t.tile == tile).type;
                    string event_name = tile_data_list.list.Single(t => t.tile == tile).event_name;
                    var rot = tilemap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation;
                    tile_data.name = name;
                    tile_data.type = type;
                    tile_data.rot = rot;
                    tile_data.event_name = event_name;
                }

                tile_datas.Add(tile_data);
            }

            list.data_list.Add(tile_datas);
        }

        string json = JsonMapper.ToJson(list);
        //Debug.Log(json);

        if(!Directory.Exists(FILE_DIR))
        {
            Directory.CreateDirectory(FILE_DIR);
        }

        string path = Path.Combine("Assets/Resources/" + FILE_DIR + FILE_NAME + ".json");

        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }

    public void Load()
    {
        if(tilemap)
        {
            tilemap.ClearAllTiles();
        }
        else
        {
            var grid = Instantiate(empty_grid_prefab);
            tilemap = grid.transform.GetChild(0).GetComponent<Tilemap>();
        }

        string json = Resources.Load(FILE_DIR + FILE_NAME).ToString();

        var json_data = JsonMapper.ToObject<MapDatas>(json);

        int y = (int)json_data.setting.origin.y;
        foreach(var data_list in json_data.data_list)
        {
            int x = (int)json_data.setting.origin.x;
            foreach(var map_data in data_list)
            {
                if(map_data.name == "")
                {
                    x++;
                    continue;
                }
                else
                {
                    if(map_data.type == "enemy")
                    {
                        Vector3 pos = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                        GenerateEnemy(map_data.name, pos);
                    }
                    else if(map_data.type == "tile")
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile_data_list.list.Single(t => t.name == map_data.name).tile);
                        Matrix4x4 mat = Matrix4x4.TRS(Vector3.zero, map_data.rot, Vector3.one);
                        tilemap.SetTransformMatrix(new Vector3Int(x, y, 0), mat);
                    }
                    else if(map_data.type == "trigger")
                    {
                        Vector3 pos = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                        GenerateTrigger(map_data.name, map_data.event_name, pos);
                    }

                    x++;
                }
            }
            y++;
        }

        //trigger_controller.InitTriggers();
    }

    void GenerateEnemy(string _name, Vector3 _pos)
    {
        GameObject obj = enemy_generator.Generate(_name, _pos);
        GameObject game_director = GameObject.Find("GameDirector");
        string uid = obj.GetComponent<EnemyBase>().GetUID();
        Debug.Log("Add::"+uid);
        game_director.GetComponent<GameDirector>().AddQuestData(uid);
    }

    void GenerateTrigger(string _name, string _event, Vector3 _pos)
    {
        trigger_controller.Generate(_name, _event, _pos);
    }
}

public class MapDatas
{
    public MapSetting setting = new MapSetting();
    public List<List<MapData>> data_list = new List<List<MapData>>();
}

public class MapSetting
{
    public Vector3 origin = new Vector3(0, 0, 0);
    public Vector3 size = new Vector3(0, 0, 0);
}

public class MapData
{
    public string name = "";
    public string type = "";
    public Quaternion rot = new Quaternion(0, 0, 0, 1.0f);
    public string event_name = "";
}