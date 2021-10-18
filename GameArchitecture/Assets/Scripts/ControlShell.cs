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
                        if(faction.Value < k.tilesOwned / 4.0f && k.money + faction.Value > 0 && Random.value < bb.factions[faction.Key].warChance)
                        {
                            bb.factions[faction.Key].factionsAtWar.Remove(k.id);
                            k.factionsAtWar.Remove(faction.Key);
                        }
                    }
                }

            }
        }
    }
}
