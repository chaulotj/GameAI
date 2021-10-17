using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShell : MonoBehaviour
{
    Blackboard bb;
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
