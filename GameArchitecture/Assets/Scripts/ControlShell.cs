using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShell : MonoBehaviour
{
    public Tile[,] tileMatrix;
    public List<Tile> tileList;
    // Start is called before the first frame update
    void Start()
    {
        tileMatrix = new Tile[36, 64];
        tileList = new List<Tile>();
        for(int c = 0; c < transform.childCount; c++)
        {
            Transform row = transform.GetChild(c);
            for(int d = 0; d < row.childCount; d++)
            {
                Tile tile = row.GetChild(d).GetComponent<Tile>();
                tileMatrix[c, d] = tile;
                tileList.Add(tile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
