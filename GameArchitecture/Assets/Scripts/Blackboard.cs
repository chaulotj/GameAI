using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public Tile[,] tileMatrix;
    public List<Tile> tileList;
    public List<KnowledgeSource> factions;
    public int factionCount = 6;
    // Start is called before the first frame update
    void Awake()
    {
        tileMatrix = new Tile[64, 36];
        tileList = new List<Tile>();
        factions = new List<KnowledgeSource>();
        for(int c = 0; c < factionCount; c++)
        {
            factions.Add(new KnowledgeSource());
        }
        for (int c = 0; c < transform.childCount; c++)
        {
            Transform row = transform.GetChild(c);
            for (int d = 0; d < row.childCount; d++)
            {
                Tile tile = row.GetChild(d).GetComponent<Tile>();
                tile.x = d;
                tile.y = c;
                tileMatrix[d, c] = tile;
                tileList.Add(tile);
            }
        }
        foreach (Tile t in tileList)
        {
            if (t.resource == Resource.None)
            {
                float f = Random.value;
                if (f < .01f)
                {
                    t.resource = Resource.Food;
                    MakeMoreResource(t, Resource.Food, .5f);
                }
                else if (f < .02f)
                {
                    t.resource = Resource.Money;
                    MakeMoreResource(t, Resource.Money, .2f);
                }
                else if (f < .03f)
                {
                    t.resource = Resource.Production;
                    MakeMoreResource(t, Resource.Production, .35f);
                }
            }
        }
        Dictionary<Vector2Int, bool> startDict = new Dictionary<Vector2Int, bool>();
        for (int c = 0; c < factionCount; c++)
        {
            factions[c].id = c;

        }
    }
    
    void MakeMoreResource(Tile tile, Resource resource, float chance)
    {
        if (tile.x < 63 && tileMatrix[tile.x + 1, tile.y].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.x + 1, tile.y].resource = resource;
            MakeMoreResource(tileMatrix[tile.x + 1, tile.y], resource, chance * .25f);
        }
        if (tile.x > 0 && tileMatrix[tile.x - 1, tile.y].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.x - 1, tile.y].resource = resource;
            MakeMoreResource(tileMatrix[tile.x - 1, tile.y], resource, chance * .25f);
        }
        if (tile.y < 35 && tileMatrix[tile.x, tile.y + 1].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.x, tile.y + 1].resource = resource;
            MakeMoreResource(tileMatrix[tile.x, tile.y + 1], resource, chance * .25f);
        }
        if (tile.y > 0 && tileMatrix[tile.x, tile.y - 1].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.x, tile.y - 1].resource = resource;
            MakeMoreResource(tileMatrix[tile.x, tile.y - 1], resource, chance * .25f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
