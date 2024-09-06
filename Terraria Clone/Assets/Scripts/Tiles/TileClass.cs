using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public TileClass wallVariant;
    public Sprite[] tileSprites;
    public int tileDropChance = 1;
    public bool inBackground = true;
    public bool naturallyPlaced = false;

    public TileClass(TileClass tile, bool isNaturallyPlaced) 
    {
        tileName = tile.name;
        wallVariant = tile.wallVariant;
        tileSprites = tile.tileSprites;
        tileDropChance = tile.tileDropChance;
        inBackground = tile.inBackground;
        naturallyPlaced = isNaturallyPlaced;
    }
}
