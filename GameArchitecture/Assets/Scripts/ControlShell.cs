using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShell : MonoBehaviour
{
    Blackboard bb;
    private bool paused;
    private Transform canvas;
    //public float secondsBetweenTurns;
    //public float timer;
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        bb = GetComponent<Blackboard>();
        canvas = GameObject.Find("Canvas").transform;
        for (int c = 0; c < bb.factionCount; c++)
        {
            while (true)
            {
                int num = Random.Range(0, bb.landTileList.Count);
                if (bb.landTileList[num].owner == -1)
                {
                    bb.factions[c].TakeTile(bb.landTileList[num]);
                    break;
                }
            }
        }
        //timer = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            if (paused)
            {
                canvas.GetChild(0).gameObject.SetActive(false);
                canvas.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                canvas.GetChild(0).gameObject.SetActive(true);
                canvas.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    public void Run()
    {
            //timer += Time.deltaTime;
            //timer -= secondsBetweenTurns;
            foreach (KnowledgeSource k in bb.factions)
            {
                k.IncrementResources();
                foreach (KeyValuePair<int, int> faction in k.factionsAtWar)
                {
                    if (faction.Value < -k.tilesOwned / 4.0f && k.money + faction.Value > 0 && Random.value < bb.factions[faction.Key].warChance)
                    {
                        k.money += faction.Value;
                        bb.factions[faction.Key].factionsAtWar.Remove(k.id);
                        k.factionsAtWar.Remove(faction.Key);
                    }
                }
                k.ChoosePriorityTiles();
            }
            int totalDone = 0;
            List<bool> done = new List<bool>() { false, false, false, false, false, false };
            List<int> indexes = new List<int>() { 0, 0, 0, 0, 0, 0 };
        int bigLoops = 0;
        int maxLoops = 0;
        foreach(KnowledgeSource k in bb.factions)
        {
            maxLoops += k.priorityTiles.Count;
        }
        Debug.Log(maxLoops);
        while (totalDone < bb.factionCount)
        {
            bigLoops++;
            foreach (KnowledgeSource k in bb.factions)
            {
                if (!done[k.id] && k.priorityTiles.Count > 0)
                {
                    int loops = 0;
                    while (true)
                    {
                        loops++;
                        Tile tile = k.priorityTiles[indexes[k.id]];
                        int moneyCost = 0;
                        int productionCost = 0;
                        int foodCost = 0;
                        if (tile.owner != -1)
                        {
                            productionCost = (bb.factions[tile.owner].productionPerTurn / 4) + 2;
                            moneyCost = (bb.factions[tile.owner].moneyPerTurn / 4) + 2;
                        }
                        if (tile.land)
                        {
                            foodCost += 2;
                        }
                        else
                        {
                            foodCost++;
                            productionCost++;
                        }
                        if (tile.resource != Resource.None)
                        {
                            productionCost++;
                        }
                        indexes[k.id]++;
                        if (moneyCost < k.money && productionCost < k.production && foodCost < k.food)
                        {
                            k.money -= moneyCost;
                            k.production -= productionCost;
                            k.food -= foodCost;
                            if (tile.owner != -1)
                            {
                                bb.factions[tile.owner].LoseTile(tile);
                            }
                            k.TakeTile(tile);
                            if (indexes[k.id] >= k.priorityTiles.Count)
                            {
                                done[k.id] = true;
                                totalDone++;
                            }
                            break;
                        }
                        else if (indexes[k.id] >= k.priorityTiles.Count)
                        {
                            done[k.id] = true;
                            totalDone++;
                            break;
                        }
                        if(loops > k.priorityTiles.Count)
                        {
                            done[k.id] = true;
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
    }
}
