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

    [SerializeField] string FILE_NAME = "map_data.json";
    [SerializeField] string FILE_DIR = "Assets/MapData/";

    // Start is called before the first frame update
    void Start()
    {
        Save();
    }

    public void Save()
    {
        // 反転回転の取得
        // TilemapのGetTransformMatrix(Vector3Int).rotation

        // 通常         Quaternion(0f,0f,0f,1f)
        // X軸反転      Quaternion(0.0, 1.0, 0.0, 0.0)
        // Y軸反転	    Quaternion(1.0, 0.0, 0.0, 0.0)
        // 90度回転     Quaternion(0.0, 0.0, -0.7, 0.7)
        // 180度回転	Quaternion(0.0, 0.0, 1.0, 0.0)
        // 270度回転	Quaternion(0.0, 0.0, 0.7, 0.7)

        tilemap.CompressBounds();
        var b = tilemap.cellBounds;

        List<MapDatas> list = new List<MapDatas>();

        for(int y = b.min.y; y < b.max.y; y++)
        {
            MapDatas tile_datas = new MapDatas();

            for(int x = b.min.x; x < b.max.x; x++)
            {
                MapData tile_data = new MapData();

                if(tilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                    string name = tile_data_list.list.Single(t => t.tile == tile).name;
                    var rot = tilemap.GetTransformMatrix(new Vector3Int(x, y, 0)).rotation;
                    tile_data.name = name;
                    tile_data.rot = rot;                  
                }

                tile_datas.data_list.Add(tile_data);
            }

            list.Add(tile_datas);
        }

        string json = JsonMapper.ToJson(list);
        //Debug.Log(json);

        if(!Directory.Exists(FILE_DIR))
        {
            Directory.CreateDirectory(FILE_DIR);
        }

        string path = Path.Combine(FILE_DIR + FILE_NAME);

        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }
}

public class MapDatas
{
    public List<MapData> data_list = new List<MapData>();
}

public class MapData
{
    public string name = "";
    public Quaternion rot = new Quaternion(0, 0, 0, 0);
}