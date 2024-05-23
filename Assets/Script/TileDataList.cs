using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileDataList", menuName = "TileDataList", order = 0)]
public class TileDataList : ScriptableObject
{
    public List<TileData> list = new List<TileData>();
}

[Serializable]
public class TileData
{
    public string name;
    public string type;
    public Tile tile;
    public string event_name;
}