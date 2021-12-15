using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControlShell : MonoBehaviour
{
    Blackboard bb;
    private bool paused;
    private Transform canvas;
    public static bool autorun;
    public float secondsBetweenTurns;
    public float timer;
    private CanvasDisplay display;
    private GameObject rulerCanvasText;
    // Start is called before the first frame update
    void Start()
    {
        display = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<CanvasDisplay>();
        rulerCanvasText = GameObject.Find("Canvas").transform.GetChild(3).gameObject;
        paused = false;
        autorun = false;
        bb = GetComponent<Blackboard>();
        canvas = GameObject.Find("Canvas").transform;
        //First I assign starting tiles for all tiles
        for (int c = 0; c < bb.factionCount; c++)
        {
            while (true)
            {
                int num = Random.Range(0, bb.landTileList.Count);
                if (bb.landTileList[num].owner == -1)
                {
                    bb.landTileList[num].IncreaseSettlementLevel();
                    bb.factions[c].TakeTile(bb.landTileList[num]);
                    break;
                }
            }
        }
        timer = 0f;
    }

    void Update()
    {
        //Pausing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            if (paused)
            {
                canvas.GetChild(0).gameObject.SetActive(false);
                canvas.GetChild(1).gameObject.SetActive(true);
                rulerCanvasText.SetActive(false);
                display.SpawnButtons();
                canvas.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                canvas.GetChild(0).gameObject.SetActive(true);
                canvas.GetChild(1).gameObject.SetActive(false);
                rulerCanvasText.SetActive(true);
                canvas.GetChild(2).gameObject.SetActive(true);
            }
        }
        //Autorunning
        if (!paused)
        {
            if (autorun)
            {
                timer += Time.deltaTime;
                if(timer > secondsBetweenTurns)
                {
                    timer -= secondsBetweenTurns;
                    Run();
                }
            }
        }
    }

    public void Rebel(Tile s, KnowledgeSource k)
    {
        //What happens when a faction rebels: They take a bunch of tiles around them
        KnowledgeSource rebel = new KnowledgeSource();
        bb.factionCount++;
        Blackboard.totalFactions++;
        rebel.id = Blackboard.totalFactions;
        rebel.Init(bb);
        bb.factions.Add(rebel.id, rebel);
        int xMin = s.pos.x - s.settlementLevel;
        if (xMin < 0)
        {
            xMin = 0;
        }
        int xMax = s.pos.x + s.settlementLevel;
        if (xMax > 63)
        {
            xMax = 63;
        }
        int yMin = s.pos.y - s.settlementLevel;
        if (yMin < 0)
        {
            yMin = 0;
        }
        int yMax = s.pos.y + s.settlementLevel;
        if (yMax > 35)
        {
            yMax = 35;
        }
        for (int c = xMin; c < xMax + 1; c++)
        {
            for (int d = yMin; d < yMax + 1; d++)
            {
                if (bb.tileMatrix[c, d].owner == k.id)
                {
                    k.LoseTile(bb.tileMatrix[c, d]);
                    rebel.TakeTile(bb.tileMatrix[c, d]);
                }
            }
        }
    }

    // Update is called once per frame
    public void Run()
    {
        foreach (KeyValuePair<int, KnowledgeSource> k in bb.factions)
        {
            //I increment all resources
            k.Value.IncrementResources();
            foreach (KeyValuePair<string, Tile> s in k.Value.settlements)
            {
                //Chance of rebelling or just losing population from lack of food
                if (k.Value.food >= s.Value.settlementLevel)
                {
                    k.Value.food -= s.Value.settlementLevel;
                }
                else
                {
                    s.Value.DecreaseSettlementLevel();
                    k.Value.food = 0;
                    if (Random.value < .1f)
                    {
                        Rebel(s.Value, k.Value);
                    }
                }
            }
            foreach (KeyValuePair<int, int> faction in k.Value.factionsAtWar)
                {
                    //All factions losing by enough offer surrender for money
                    if (faction.Value < -k.Value.tilesOwned / 4.0f && (2*k.Value.money) + faction.Value > 0 && Random.value < bb.factions[faction.Key].warChance)
                    {
                        k.Value.money -= faction.Value/2;
                        bb.factions[faction.Key].money += faction.Value / 2;
                        bb.factions[faction.Key].factionsAtWar.Remove(k.Value.id);
                        k.Value.factionsAtWar.Remove(faction.Key);
                    }
                }
                //Sort the priority tiles of all knowledge sources
                k.Value.ChoosePriorityTiles();
            }
        int totalDone = 0;
        Dictionary<int, bool> done = new Dictionary<int, bool>();
        Dictionary<int, int> indexes = new Dictionary<int, int>();
        foreach (KeyValuePair<int, KnowledgeSource> k in bb.factions)
        {
            done.Add(k.Value.id, false);
            indexes.Add(k.Value.id, 0);
        }
        int bigLoops = 0;
        int maxLoops = 0;
        foreach (KeyValuePair<int, KnowledgeSource> k in bb.factions)
        {
            maxLoops += k.Value.priorityTiles.Count;
        }
        while (totalDone < bb.factionCount)
        {
            bigLoops++;
            foreach (KeyValuePair<int, KnowledgeSource> k in bb.factions)
            {
                if (!done[k.Value.id] && k.Value.priorityTiles.Count > 0)
                {
                    //Each faction's expansion
                    int loops = 0;
                    while (true)
                    {
                        loops++;
                        Tile tile = k.Value.priorityTiles[indexes[k.Value.id]];
                        int moneyCost = 0;
                        int productionCost = 0;
                        int foodCost = 0;
                        //If they're trying to capture the tile, adds to production/money cost
                        if (tile.owner != -1)
                        {
                            productionCost += (int)(((bb.factions[tile.owner].productionPerTurn / 2) + 2) / k.Value.rulerWarSkill * bb.factions[tile.owner].rulerWarSkill);
                            moneyCost += (int)(((bb.factions[tile.owner].moneyPerTurn / 2) + 2) / k.Value.rulerWarSkill * bb.factions[tile.owner].rulerWarSkill);
                            if(tile.settlementLevel > -1)
                            {
                                productionCost += tile.settlementLevel + 1;
                                moneyCost += tile.settlementLevel + 1;
                            }
                        }
                        if (tile.land) //Cost for land tile
                        {
                            foodCost += 28;
                        }
                        else //Cost for sea tile
                        {
                            foodCost += 20;
                            productionCost += 8;
                        }
                        if (tile.resource != Resource.None) //Cost to take a resource tile
                        {
                            productionCost += 8;
                            moneyCost += 8;
                        }
                        indexes[k.Value.id]++;
                        if (moneyCost < k.Value.money && productionCost < k.Value.production && foodCost < k.Value.food)
                        {
                            //If they can afford the tile, they take it
                            k.Value.money -= moneyCost;
                            k.Value.production -= productionCost;
                            k.Value.food -= foodCost;
                            if (tile.owner != -1)
                            {
                                bb.factions[tile.owner].LoseTile(tile);
                            }
                            k.Value.TakeTile(tile);
                            if (indexes[k.Value.id] >= k.Value.priorityTiles.Count)
                            {
                                done[k.Value.id] = true;
                                totalDone++;
                            }
                            break;
                        }
                        else if (indexes[k.Value.id] >= k.Value.priorityTiles.Count)
                        {
                            //If their priorityTiles list is done
                            done[k.Value.id] = true;
                            totalDone++;
                            break;
                        }
                        if(loops > k.Value.priorityTiles.Count)
                        {
                            done[k.Value.id] = true;
                            totalDone++;
                            break;
                        }
                    }
                }
            }
            if(bigLoops > maxLoops)
            {
                break;
            }
        }
        foreach (KeyValuePair<int, KnowledgeSource> k in bb.factions)
        {
            foreach (KeyValuePair<string, Tile> s in k.Value.settlements)
            {
                //Chance of rebelling due to unpopularity
                if (Random.value < k.Value.rulerUnpopularity)
                {
                    Rebel(s.Value, k.Value);
                }
            }
            if (k.Value.food > 4 && k.Value.money > 4 && k.Value.production > 4 && Random.value < .1f)
            {
                //Building settlements
                if(k.Value.numSettlements < k.Value.tilesOwned / 40)
                {
                    k.Value.BuildSettlement(k.Value.landTiles[Random.Range(0, k.Value.landTiles.Count)]);
                    k.Value.food -= 5;
                    k.Value.money -= 5;
                    k.Value.production -= 5;
                }
                else if(k.Value.nonTopLevelSettlements.Count > 0)
                {
                    k.Value.BuildSettlement(k.Value.nonTopLevelSettlements.ElementAt(Random.Range(0, k.Value.nonTopLevelSettlements.Count)).Value);
                    k.Value.food -= 5;
                    k.Value.money -= 5;
                    k.Value.production -= 5;
                }
            }
            if (Random.value < k.Value.rulerPoorHealth)
            {
                //Leader death
                k.Value.ChangeRuler();
            }
        }
    }
}
