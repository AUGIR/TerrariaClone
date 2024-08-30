using System.Collections;
using System.Collections.Generic;
using UnityEditor.Android;
using UnityEngine;

[System.Serializable]
public class BiomeClass
{
    public string biomeName;
    public Color biomeCol;
    public TileAtlas tileAtlas;

    [Header("World Settings")]
    [Range(0f, 100f)] public float heightMultiplier = 4f;

    [Header("Generation Settings")]
    public bool generateFlatBiome;
    public bool generateCaves;
    [Range(0f, 1f)] public float surfaceValue = 0.2f;
    [Range(0f, 30f)] public int dirtLayerHeight = 5;

    [Header("Noise Settings")]
    [Range(0f, 0.15f)] public float terrainFreq = 0.04f;
    [Range(0f, 0.15f)] public float caveFreq = 0.05f;
    public Texture2D caveNoiseTexture;

    [Header("Tree Settings")]
    [Range(0f, 100f)] public int treeChance = 10;
    [Range(0f, 30f)] public int minTreeHeight = 4;
    [Range(0f, 30f)] public int maxTreeHeight = 14;

    [Header("Visual Addons")]
    public int tallGrassChance = 20;
    public int cactusChance;
    public int minCactusHeight;
    public int maxCactusHeight;

    [Header("Ore Settings")]
    public OreClass[] ores;



}
