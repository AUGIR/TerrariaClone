using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public PlayerController player;
    public BiomeClass[] biomes;
    public Camera cam;
    public GameObject tileDrop;

    [Header("General World Settings")]
    [Range(0f, 10000f)] public int worldSize = 100;
    [Range(0f, 100f)] public float heightMultiplier = 4f;
    [Range(0f, 500f)] public int heightAddition = 25;
    public int chunkSize = 16;
    private GameObject[] worldChunks;

    [Header("Biomes")]
    public float biomeFrequency;
    public Gradient biomeGradient;
    [HideInInspector]
    public Texture2D biomeMap;

    public bool generateFlatBiomes;

    [Header("Generation Settings")]
    [Range(-10000f, 10000f)] public float seed;
    public bool generateRandomSeed;
    public bool generateCaves;
    private List<Vector2> worldTiles = new List<Vector2>();
    private List<GameObject> worldTileObjects = new List<GameObject>();
    private List<TileClass> worldTileClasses = new List<TileClass>();

    [Header("Noise Settings")]
    [HideInInspector]
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
        player.Spawn();

    }

    public void RefreshChunks()
    {
        for (int i = 0; i < worldChunks.Length; i++)
        {
            if (Vector2.Distance(new Vector2((i * chunkSize) + (chunkSize / 2), 0), new Vector2(player.transform.position.x, 0)) > cam.orthographicSize * 4f)
            {
                worldChunks[i].SetActive(false);
            }
            else
            {
                worldChunks[i].SetActive(true);
            }
        }
    }

    public void Update()
    {
        RefreshChunks();
    }

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
        TileClass tileClass;
        for (int x = 0; x < worldSize; x++)
        {

            float height;
            for (int y = 0; y < worldSize; y++)
            {
                curBiome = GetCurrentBiome(x, y);
                height = Mathf.PerlinNoise((x + seed) * curBiome.terrainFreq, seed * curBiome.terrainFreq) * curBiome.heightMultiplier + heightAddition;

                if (x == worldSize / 2)
                {
                    player.spawnPos = new Vector2(x, height + 2);
                }

                if (y >= height)
                    break;
                    

                    if (y < height - curBiome.dirtLayerHeight)
                    {

                        tileClass =  curBiome.tileAtlas.stone;
                        if (curBiome.ores[4].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[4].maxSpawnHeight)
                        {
                            tileClass = curBiome.tileAtlas.dirt;
                        }
                        if (curBiome.ores[0].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[0].maxSpawnHeight)
                        {
                            tileClass = tileAtlas.coal;
                        }
                        if (curBiome.ores[1].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[1].maxSpawnHeight)
                        {
                            tileClass = tileAtlas.iron;
                        }
                        if (curBiome.ores[2].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[2].maxSpawnHeight)
                        {
                            tileClass = tileAtlas.gold;
                        }
                        if (curBiome.ores[3].spreadTexture.GetPixel(x, y).r > 0.5f && height - y > curBiome.ores[3].maxSpawnHeight)
                        {
                            tileClass = tileAtlas.diamond;
                        }



                    }
                    else if (y < height - 1)
                    {
                        tileClass = curBiome.tileAtlas.dirt;
                    }
                    else
                    {
                        tileClass = curBiome.tileAtlas.grass;

                    }

                    if (generateCaves)
                    {
                        if (caveNoiseTexture.GetPixel(x, y).r > 0.5f)
                        {
                            PlaceTile(tileClass, x, y, true);
                        }
                        else if(tileClass.wallVariant != null)
                        {
                            PlaceTile(tileClass.wallVariant, x, y, true);
                        }
                    }
                    else
                    {
                        PlaceTile(tileClass, x, y, true);
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
                                    PlaceTile(curBiome.tileAtlas.tallGrass, x, y + 1, true);
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
            PlaceTile(this.tileAtlas.log, x, y + i, true);
        }

        PlaceTile(tileAtlas.leaf, x, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, x, y + treeHeight + 2, true);
        PlaceTile(tileAtlas.leaf, x, y + treeHeight + 3, true);
        PlaceTile(tileAtlas.leaf, x, y + treeHeight + 4, true);

        PlaceTile(tileAtlas.leaf, x + 1, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, x + 2, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, x + 3, y + treeHeight + 1, true);

        PlaceTile(tileAtlas.leaf, x + 1, y + treeHeight + 2, true);
        PlaceTile(tileAtlas.leaf, x + 2, y + treeHeight + 2, true);

        PlaceTile(tileAtlas.leaf, x + 1, y + treeHeight + 3, true);

        PlaceTile(tileAtlas.leaf, x - 1, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, x - 2, y + treeHeight + 1, true);
        PlaceTile(tileAtlas.leaf, x - 3, y + treeHeight + 1, true);

        PlaceTile(tileAtlas.leaf, x - 1, y + treeHeight + 2, true);
        PlaceTile(tileAtlas.leaf, x - 2, y + treeHeight + 2, true);

        PlaceTile(tileAtlas.leaf, x - 1, y + treeHeight + 3, true);

    }

    public void GenerateCactus(int cactusHeight, int x, int y)
    {
        for (int i = 0; i <= cactusHeight; i++)
        {
            PlaceTile(cactus.cactus, x, y + i, true);
        }

        PlaceTile(cactus.cactusTop, x, y + cactusHeight + 1, true);

    }

    public void RemoveTile(int x, int y)
    {
        TileClass tile = worldTileClasses[worldTiles.IndexOf(new Vector2(x, y))];
        if (worldTiles.Contains(new Vector2Int(x, y)) && x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {

            if (tile.wallVariant != null)
            {
                if (tile.naturallyPlaced)
                {
                    PlaceTile(tile.wallVariant, x, y, true);
                }
               
            }
            int t = Random.Range(1, tile.tileDropChance);
            Destroy(worldTileObjects[worldTiles.IndexOf(new Vector2(x, y))]);
            if (t == 1)
            {
                GameObject newTileDrop = Instantiate(tileDrop, new Vector2(x, y + 1f), Quaternion.identity);
                newTileDrop.GetComponent<SpriteRenderer>().sprite = tile.tileSprites[0];
            }
            
            worldTileObjects.RemoveAt(worldTiles.IndexOf(new Vector2(x, y)));
            worldTileClasses.RemoveAt(worldTiles.IndexOf(new Vector2(x, y)));
            worldTiles.RemoveAt(worldTiles.IndexOf(new Vector2(x, y)));

        }
    }

    public void CheckTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        if (x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {
            if (!worldTiles.Contains(new Vector2Int(x, y)))
            {
                PlaceTile(tile, x, y, isNaturallyPlaced);
            }
            else
            {
                if (worldTileClasses[worldTiles.IndexOf(new Vector2Int(x, y))].inBackground)
                {
                    RemoveTile(x, y);
                    PlaceTile(tile, x, y, isNaturallyPlaced);
                }
            }
        }
    }

    public void PlaceTile(TileClass tile, int x, int y, bool isNaturallyPlaced)
    {
        bool backgroundElement = tile.inBackground;

        if (x >= 0 && x <= worldSize && y >= 0 && y <= worldSize)
        {

            GameObject newTile = new GameObject();

            int chunkCoord = Mathf.RoundToInt(Mathf.Round(x / chunkSize) * chunkSize);
            chunkCoord /= chunkSize;

            newTile.transform.parent = worldChunks[(int)chunkCoord].transform;

            newTile.AddComponent<SpriteRenderer>();
            if (!backgroundElement)
            {
                newTile.AddComponent<BoxCollider2D>();
                newTile.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
                newTile.tag = "Ground";
            }

            if (tile.inBackground)
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -10;
            }
            else
            {
                newTile.GetComponent<SpriteRenderer>().sortingOrder = -5;
            }

            if (tile.name.ToUpper().Contains("WALL"))
            {
                newTile.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            }

            int spriteIndex = Random.Range(0, tile.tileSprites.Length);
            newTile.GetComponent<SpriteRenderer>().sprite = tile.tileSprites[spriteIndex];
            newTile.name = tile.tileSprites[0].name;
            newTile.transform.position = new Vector2(x + 0.5f, y + 0.5f);
            tile.naturallyPlaced = isNaturallyPlaced;

            worldTiles.Add(newTile.transform.position - (Vector3.one * 0.5f));
            worldTileObjects.Add(newTile);
            worldTileClasses.Add(tile);
        }
    }
}
