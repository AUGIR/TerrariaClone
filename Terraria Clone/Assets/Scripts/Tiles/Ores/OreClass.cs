using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OreClass
{
    public string name;
    [Range(-1f, 1f)]
    public float rarity;
    [Range (0f, 1f)]
    public float size;
    [Range (0f, 500f)]
    public int maxSpawnHeight;
    public Texture2D spreadTexture;
}
