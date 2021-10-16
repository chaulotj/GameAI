using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnowledgeSource : MonoBehaviour
{
    public int id;
    public int food;
    public int foodPerTurn;
    public int money;
    public int moneyPerTurn;
    public int production;
    public int productionPerTurn;
    public Dictionary<Vector2Int, Tile> ownedTiles;
    public Dictionary<Vector2Int, Tile> expandableTiles;

    public void TakeTile(Tile tile)
    {
        tile.owner = id;
    }

    public void UpdateBlackBoard()
    {

    }
    // Start is called before the first frame update
    void Awake()
    {
        food = 1;
        foodPerTurn = 1;
        money = 1;
        moneyPerTurn = 1;
        production = 1;
        productionPerTurn = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
