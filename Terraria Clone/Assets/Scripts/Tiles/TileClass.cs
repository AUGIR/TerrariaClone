using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newtileclass", menuName = "Tile Class")]
public class TileClass : ScriptableObject
{
    public string tileName;
    public TileClass wallVariant;
    public Sprite[] tileSprites;
    public Sprite tileDrop;
    public bool inBackground = true;
    public bool naturallyPlaced = false;

    public static TileClass CreateInstance(TileClass tile, bool isNaturallyPlaced) 
    {
        var thisTile = ScriptableObject.CreateInstance<TileClass>();

        thisTile.Init(tile, isNaturallyPlaced);

        return thisTile;
    }

    public void Init(TileClass tile, bool isNaturallyPlaced)
    {

        tileName = tile.name;
        wallVariant = tile.wallVariant;
        tileSprites = tile.tileSprites;
        tileDrop = tile.tileDrop;
        inBackground = tile.inBackground;
        naturallyPlaced = isNaturallyPlaced;
    }
}
