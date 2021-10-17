using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public Tile[,] tileMatrix;
    public List<Tile> tileList;
    public List<KnowledgeSource> factions;
    public int factionCount = 6;
    public List<Tile> landTileList;
    public static int totalTilesOwned; //Once this reaches 2304, it's gg
    // Start is called before the first frame update
    void Awake()
    {
        tileMatrix = new Tile[64, 36];
        tileList = new List<Tile>();
        factions = new List<KnowledgeSource>();
        landTileList = new List<Tile>();
        totalTilesOwned = 0;
        for(int c = 0; c < factionCount; c++)
        {
            KnowledgeSource k = new KnowledgeSource();
            k.id = c;
            k.Init(this);
            factions.Add(k);
        }
        for (int c = 0; c < transform.childCount; c++)
        {
            Transform row = transform.GetChild(c);
            for (int d = 0; d < row.childCount; d++)
            {
                Tile tile = row.GetChild(d).GetComponent<Tile>();
                tile.pos = new Vector2Int(d, c);
                tileMatrix[d, c] = tile;
                tileList.Add(tile);
                if (tile.land)
                {
                    landTileList.Add(tile);
                }
            }
        }
        foreach (Tile t in tileList)
        {
            if (t.resource == Resource.None)
            {
                float f = Random.value;
                if (f < .015f)
                {
                    t.resource = Resource.Food;
                    MakeMoreResource(t, Resource.Food, .5f);
                }
                else if (f < .025f)
                {
                    t.resource = Resource.Money;
                    MakeMoreResource(t, Resource.Money, .2f);
                }
                else if (f < .035f)
                {
                    t.resource = Resource.Production;
                    MakeMoreResource(t, Resource.Production, .35f);
                }
            }
        }
    }
    
    void MakeMoreResource(Tile tile, Resource resource, float chance)
    {
        if (tile.pos.x < 63 && tileMatrix[tile.pos.x + 1, tile.pos.y].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x + 1, tile.pos.y].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x + 1, tile.pos.y], resource, chance * .25f);
        }
        if (tile.pos.x > 0 && tileMatrix[tile.pos.x - 1, tile.pos.y].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x - 1, tile.pos.y].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x - 1, tile.pos.y], resource, chance * .25f);
        }
        if (tile.pos.y < 35 && tileMatrix[tile.pos.x, tile.pos.y + 1].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x, tile.pos.y + 1].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x, tile.pos.y + 1], resource, chance * .25f);
        }
        if (tile.pos.y > 0 && tileMatrix[tile.pos.x, tile.pos.y - 1].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x, tile.pos.y - 1].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x, tile.pos.y - 1], resource, chance * .25f);
        }
    }
}
