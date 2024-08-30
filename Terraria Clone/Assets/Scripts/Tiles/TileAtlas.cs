using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileAtlas", menuName = "Tile Atlas")]
public class TileAtlas : ScriptableObject
{

    [Header("Ore")]
    public TileClass coal;
    public TileClass iron;
    public TileClass gold;
    public TileClass diamond;
    public TileClass tallGrass;

    [Header("Plains/Forest/Basic Blocks")]
    public TileClass stone;
    public TileClass dirt;
    public TileClass grass;
    public TileClass log;
    public TileClass leaf;

    [Header("Snow Biome")]
    public TileClass snow;

    [Header("Desert Biome")]
    public TileClass sand;


}
