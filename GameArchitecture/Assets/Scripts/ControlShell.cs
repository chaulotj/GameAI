using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShell : MonoBehaviour
{
    Blackboard bb;
    public float secondsBetweenTurns;
    public float timer;
    public bool done;
    // Start is called before the first frame update
    void Start()
    {
        bb = GetComponent<Blackboard>();
        for (int c = 0; c < bb.factionCount; c++)
        {
            while (true)
            {
                int num = Random.Range(0, bb.landTileList.Count);
                if (bb.landTileList[num].owner == 0)
                {
                    bb.factions[c].TakeTile(bb.landTileList[num]);
                    break;
                }
            }
        }
        timer = 0f;
        done = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            timer += Time.deltaTime;
            if (timer > secondsBetweenTurns)
            {
                timer -= secondsBetweenTurns;
                foreach (KnowledgeSource k in bb.factions)
                {
                    k.IncrementResources();
                    foreach (KeyValuePair<int, int> faction in k.factionsAtWar)
                    {
                        if(faction.Value < -k.tilesOwned / 4.0f && k.money + faction.Value > 0 && Random.value < bb.factions[faction.Key].warChance)
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
                while (totalDone < bb.factionCount)
                {
                    foreach (KnowledgeSource k in bb.factions)
                    {
                        if (!done[k.id])
                        {
                            while (true)
                            {
                                Tile tile = k.priorityTiles[indexes[k.id]];
                                int moneyCost = 0;
                                int productionCost = 0;
                                int foodCost = 0;
                                if (tile.owner != 0)
                                {
                                    productionCost += 2;
                                    moneyCost += 2;
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
                                    if (tile.owner != 0)
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
                            }
                        }
                    }
                }
            }
        }
    }
}
