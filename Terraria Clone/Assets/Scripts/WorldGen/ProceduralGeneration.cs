using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public BiomeClass[] biomes;
    public bool syncBiomeOres;

    [Header("General World Settings")]
    [Range(0f, 10000f)] public int worldSize = 100;
    [Range(0f, 100f)] public float heightMultiplier = 4f;
    [Range(0f, 500f)] public int heightAddition = 25;
    public int chunkSize = 16;
    private GameObject[] worldChunks;

    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;
    public Texture2D biomeMap;
    public bool generateFlatBiomes;

    [Header("Generation Settings")]
    [Range(-10000f, 10000f)] public float seed;
    public bool generateRandomSeed;
    public bool generateCaves;
    private List<Vector2> worldTiles = new List<Vector2>();

    [Header("Noise Settings")]
    public Texture2D caveNoiseTexture;

    [Header("Tile Atlas")]
    public TileAtlas tileAtlas;
    public Cactus cactus;
    private BiomeClass curBiome;

    private Color[] biomeCols;

    private void OnValidate()
    {
        if (generateRandomSeed)
        {
            seed = Random.Range(-10000, 10000);
        }








        biomeCols = new Color[biomes.Length];
        for (int i = 0; i < biomes.Length; i++)
        {
            biomeCols[i] = biomes[i].biomeCol;
        }
        DrawTextures();
        DrawBiomeTexture();

        DrawCaves();

    }

    private void Start()
    {
        if (generateRandomSeed)
        {
            seed = Random.Range(-10000, 10000);
        }

        DrawTextures();

        DrawBiomeTexture();

        DrawCaves();


        CreateChunks();
        GenerateTerrain();

    }

/*    public void DrawBiomeMap()
    {
        float b;
        Color col;

        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                b = Mathf.PerlinNoise((x + seed) * biomeFrequency, seed * biomeFrequency);
                col = biomeGradient.Evaluate(b);
                biomeMap.SetPixel(x, y, col);
            }
        }


        biomeMap.Apply();
    }*/

    public void DrawCaves()
    {
        for (int x = 0; x < worldSize; x++)
        {
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);

                float v = Mathf.PerlinNoise((x + seed) * curBiome.caveFreq, (y + seed) * curBiome.caveFreq);
                if (v > curBiome.surfaceValue)
                {
                    caveNoiseTexture.SetPixel(x, y, Color.white);
                    curBiome.caveNoiseTexture.SetPixel(x, y, Color.white);
                }
                else
                {
                    caveNoiseTexture.SetPixel(x, y, Color.black);
                    curBiome.caveNoiseTexture.SetPixel(x, y, Color.black);
                }
            }
        }

        caveNoiseTexture.Apply();
        curBiome.caveNoiseTexture.Apply();

    }

    public void DrawBiomeTexture()
    {

        for (int x = 0; x < biomeMap.width; x++)
        {
            for (int y = 0; y < biomeMap.height; y++)
            {
                if (generateFlatBiomes)
                {
                    float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, seed * biomeFrequency);
                    Color col = biomeGradient.Evaluate(v);
                    biomeMap.SetPixel(x, y, col);
                }
                else
                {

                    float v = Mathf.PerlinNoise((x + seed) * biomeFrequency, (y + seed) * biomeFrequency);
                    Color col = biomeGradient.Evaluate(v);
                    biomeMap.SetPixel(x, y, col);
                }

            }
        }
        biomeMap.Apply();
    }

    public BiomeClass GetCurrentBiome(int x, int y)
    {

        if(System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y)) >= 0)
        {
            return biomes[System.Array.IndexOf(biomeCols, biomeMap.GetPixel(x, y))];
        }

        return curBiome;
    }

    public void CreateChunks()
    {
        int numChunks = worldSize / chunkSize;
        worldChunks = new GameObject[numChunks];
        for (int i = 0; i < numChunks; i++)
        {
            GameObject newChunk = new GameObject();
            newChunk.name = i.ToString();
            newChunk.transform.parent = this.transform;
            worldChunks[i] = newChunk;
        }
    }

    public void DrawTextures()
    {
        biomeMap = new Texture2D(worldSize, worldSize);
        caveNoiseTexture = new Texture2D(worldSize, worldSize);

        for (int i = 0; i < biomes.Length; i++)
        {
            biomes[i].caveNoiseTexture = new Texture2D(worldSize, worldSize);

            for (int o = 0; o < biomes[i].ores.Length; o++)
            {
                biomes[i].ores[o].spreadTexture = new Texture2D(worldSize, worldSize);
            }

            GenerateNoiseTexture(biomes[i].caveFreq, biomes[i].surfaceValue, biomes[i].caveNoiseTexture);

            for (int o = 0; o < biomes[i].ores.Length; o++)
            {
                GenerateNoiseTexture(biomes[i].ores[o].rarity, biomes[i].ores[o].size, biomes[i].ores[o].spreadTexture);
            }



        }
    }

    public void GenerateNoiseTexture(float frequency, float limit, Texture2D noiseTexture)
    {
        float v;


        for (int x = 0; x < noiseTexture.width; x++)
        {
            for (int y = 0; y < noiseTexture.height; y++)
            {
                v = Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency);


                if (v > limit)
                {
                    noiseTexture.SetPixel(x, y, Color.white);
                }
                else
                {
                    noiseTexture.SetPixel(x, y, Color.black);
                }
            }
        }
        noiseTexture.Apply();
    }

    public void GenerateTerrain()
    {
        Sprite[] tileSprites;
        for (int x = 0; x < worldSize; x++)
        {

            float height;
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;

                if (y >= height)
                    break;
                    

                    if (y < height - curBiome.dirtLayerHeight)
                    {

                        tileSprites =  curBiome.tileAtlas.stone.tileSprites;
                        if (curBiome.ores[4].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[4].maxSpawnHeight)
                        {
                            tileSprites = curBiome.tileAtlas.dirt.tileSprites;
                        }
                        if (curBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[0].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.coal.tileSprites;
                        }
                        if (curBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[1].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.iron.tileSprites;
                        }
                        if (curBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[2].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.gold.tileSprites;
                        }
                        if (curBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[3].maxSpawnHeight)
                        {
                            tileSprites = tileAtlas.diamond.tileSprites;
                        }



                    }
                    else if (y < height - 1)
                    {
                        tileSprites = curBiome.tileAtlas.dirt.tileSprites;
                    }
                    else
                    {
                        tileSprites = curBiome.tileAtlas.grass.tileSprites;

                    }

                    if (generateCaves)
                    {
                        if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                        {
                            PlaceTile(tileSprites, x, y);
                        }
                    }
                    else
                    {
                        PlaceTile(tileSprites, x, y);
                    }

                    if (y >= height - 1)
                    {
                        int tgc = Random.Range(0, curBiome.tallGrassChance);
                        if (worldTiles.Contains(new Vector2(x, y)))
                        {
                            int t = Random.Range(0, curBiome.treeChance);
                            if (t == 1)
                            {
                                GenerateTree(Random.Range(curBiome.minTreeHeight, curBiome.maxTreeHeight), x, y + 1);
                            }
                        }
                        if(tgc == 1)
                        {
                            if (worldTiles.Contains(new Vector2(x, y)))
                            {
                                if (curBiome.tileAtlas.tallGrass != null)
                                {
                                    PlaceTile(curBiome.tileAtlas.tallGrass.tileSprites, x, y + 1);
                                }

                            }

                        }
                        if (curBiome.biomeName == "Desert")
                        {
                            int t = Random.Range(0, curBiome.cactusChance);
                            if (worldTiles.Contains(new Vector2(x, y)))
                            {
                                if (t == 1)
                                {
                                    GenerateCactus(Random.Range(curBiome.minCactusHeight, curBiome.maxCactusHeight), x, y);
                                }
                            }
                    
                        }
                        

                    }

                
            }
        }
    }
    
    public void GenerateTree(int treeHeight, int x, int y)
    {
        for (int i = 0; i <= treeHeight; i++)
        {
            PlaceTile(this.tileAtlas.log.tileSprites, x, y + i);
        }

        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 2);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 3);
        PlaceTile(tileAtlas.leaf.tileSprites, x, y + treeHeight + 4);

        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x + 2, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x + 3, y + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 2);
        PlaceTile(tileAtlas.leaf.tileSprites, x + 2, y + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprites, x + 1, y + treeHeight + 3);

        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x - 2, y + treeHeight + 1);
        PlaceTile(tileAtlas.leaf.tileSprites, x - 3, y + treeHeight + 1);

        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 2);
        PlaceTile(tileAtlas.leaf.tileSprites, x - 2, y + treeHeight + 2);

        PlaceTile(tileAtlas.leaf.tileSprites, x - 1, y + treeHeight + 3);

    }

    public void GenerateCactus(int cactusHeight, int x, int y)
    {
        for (int i = 0; i <= cactusHeight; i++)
        {
            PlaceTile(cactus.cactus.tileSprites, x, y + i);
        }

        PlaceTile(cactus.cactusTop.tileSprites, x, y + cactusHeight + 1);

    }

    public void PlaceTile(Sprite[] tileSprites, int x, int y)
    {
        if (!worldTiles.Contains(new Vector2Int(x, y)))
        {
            GameObject newTile = new GameObject();

            int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;

            newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();
            int spriteIndex = Random.Range(0, tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tileSprites[spriteIndex];
            newTile.name = tileSprites[0].name;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);

            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));

        }
    }
}
