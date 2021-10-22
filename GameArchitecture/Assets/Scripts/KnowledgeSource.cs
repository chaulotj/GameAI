using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MovementType
{
    Land,
    Sea
}

public class KnowledgeSource
{
    public int id;
    public int food;
    public int foodPerTurn;
    public int money;
    public int moneyPerTurn;
    public int production;
    public int productionPerTurn;
    public Dictionary<Tile, int> ownedTiles;
    public Dictionary<Tile, int> expandableTiles;
    public Dictionary<Tile, bool> stillOwned;
    private Blackboard bb;
    private Color color;
    public Resource[] resourcePriorities; //0: first priority, 1: second priority, 2: third priority
    public float warChance; //between 0 and 1; how likely this faction is to go to war
    public MovementType preferredMovement; //Land or sea?
    public Dictionary<int, int> factionsAtWar; //owner, then warscore
    public List<Tile> priorityTiles;
    public int tilesOwned;

    public void IncrementResources()
    {
        //Called each turn
        food += foodPerTurn;
        money += moneyPerTurn;
        production += productionPerTurn;
    }

    public void TakeTile(Tile tile)
    {
        //All the steps needed to take a particular tile
        if(tile.owner == -1)
        {
            Blackboard.totalTilesOwned++;
        }
        tile.owner = id;
        tilesOwned++;
        ownedTiles.Add(tile, 1);
        //Adding the proper resource
        switch (tile.resource)
        {
            case Resource.Food:
                foodPerTurn += 2;
                break;
            case Resource.Money:
                moneyPerTurn += 2;
                break;
            case Resource.Production:
                productionPerTurn += 2;
                break;
            default:
                break;
        }
        tile.transform.GetChild(0).gameObject.SetActive(true);
        tile.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
        //All the below is to keep track of which tiles can be expanded to in the future
        if (expandableTiles.ContainsKey(tile))
        {
            expandableTiles[tile]--;
            if(expandableTiles[tile] == 0)
            {
                expandableTiles.Remove(tile);
            }
        }
        if (!stillOwned.ContainsKey(tile))
        {
            stillOwned.Add(tile, true);
        }
        int[] checkOrder = new int[4] { 0, 1, 2, 3 };
        for(int c = 0; c < 4; c++)
        {
            int target = Random.Range(0, 4);
            int tmp = checkOrder[c];
            checkOrder[c] = checkOrder[target];
            checkOrder[target] = tmp;
        }
        for(int c = 0; c < 4; c++)
        {
            switch (checkOrder[c])
            {
                case 0:
                    if (tile.pos.x < 63 && bb.tileMatrix[tile.pos.x + 1, tile.pos.y].owner != id)
                    {
                        if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x + 1, tile.pos.y]))
                        {
                            expandableTiles[bb.tileMatrix[tile.pos.x + 1, tile.pos.y]]++;
                        }
                        else
                        {
                            expandableTiles.Add(bb.tileMatrix[tile.pos.x + 1, tile.pos.y], 1);
                        }
                    }
                    break;
                case 1:
                    if (tile.pos.x > 0 && bb.tileMatrix[tile.pos.x - 1, tile.pos.y].owner != id)
                    {
                        if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x - 1, tile.pos.y]))
                        {
                            expandableTiles[bb.tileMatrix[tile.pos.x - 1, tile.pos.y]]++;
                        }
                        else
                        {
                            expandableTiles.Add(bb.tileMatrix[tile.pos.x - 1, tile.pos.y], 1);
                        }
                    }
                    break;
                case 2:
                    if (tile.pos.y < 35 && bb.tileMatrix[tile.pos.x, tile.pos.y + 1].owner != id)
                    {
                        if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x, tile.pos.y + 1]))
                        {
                            expandableTiles[bb.tileMatrix[tile.pos.x, tile.pos.y + 1]]++;
                        }
                        else
                        {
                            expandableTiles.Add(bb.tileMatrix[tile.pos.x, tile.pos.y + 1], 1);
                        }
                    }
                    break;
                case 3:
                    if (tile.pos.y > 0 && bb.tileMatrix[tile.pos.x, tile.pos.y - 1].owner != id)
                    {
                        if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x, tile.pos.y - 1]))
                        {
                            expandableTiles[bb.tileMatrix[tile.pos.x, tile.pos.y - 1]]++;
                        }
                        else
                        {
                            expandableTiles.Add(bb.tileMatrix[tile.pos.x, tile.pos.y - 1], 1);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        //if (tile.pos.x < 63 && bb.tileMatrix[tile.pos.x + 1, tile.pos.y].owner != id)
        //{
        //    if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x + 1, tile.pos.y]))
        //    {
        //        expandableTiles[bb.tileMatrix[tile.pos.x + 1, tile.pos.y]]++;
        //    }
        //    else
        //    {
        //        expandableTiles.Add(bb.tileMatrix[tile.pos.x + 1, tile.pos.y], 1);
        //    }
        //}
        //if (tile.pos.x > 0 && bb.tileMatrix[tile.pos.x - 1, tile.pos.y].owner != id)
        //{
        //    if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x - 1, tile.pos.y]))
        //    {
        //        expandableTiles[bb.tileMatrix[tile.pos.x - 1, tile.pos.y]]++;
        //    }
        //    else
        //    {
        //        expandableTiles.Add(bb.tileMatrix[tile.pos.x - 1, tile.pos.y], 1);
        //    }
        //}
        //if (tile.pos.y < 35 && bb.tileMatrix[tile.pos.x, tile.pos.y + 1].owner != id)
        //{
        //    if (expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x, tile.pos.y + 1]))
        //    {
        //        expandableTiles[bb.tileMatrix[tile.pos.x, tile.pos.y + 1]]++;
        //    }
        //    else
        //    {
        //        expandableTiles.Add(bb.tileMatrix[tile.pos.x, tile.pos.y + 1], 1);
        //    }
        //}
        //if (tile.pos.y > 0 && bb.tileMatrix[tile.pos.x, tile.pos.y - 1].owner != id)
        //{
        //    if(expandableTiles.ContainsKey(bb.tileMatrix[tile.pos.x, tile.pos.y - 1]))
        //    {
        //        expandableTiles[bb.tileMatrix[tile.pos.x, tile.pos.y - 1]]++;
        //    }
        //    else
        //    {
        //        expandableTiles.Add(bb.tileMatrix[tile.pos.x, tile.pos.y - 1], 1);
        //    }
        //}
    }

    public void LoseTile(Tile tile)
    {
        //When this faction loses a tile
        ownedTiles.Remove(tile);
        stillOwned[tile] = false;
        tilesOwned--;
        //Losing resources
        switch (tile.resource)
        {
            case Resource.Food:
                foodPerTurn -= 2;
                break;
            case Resource.Money:
                moneyPerTurn -= 2;
                break;
            case Resource.Production:
                productionPerTurn -= 2;
                break;
            default:
                break;
        }
        //Removing the propert tiles from expandableTiles
        if (tile.pos.x < 63)
        {
            CheckExpandRemoval(bb.tileMatrix[tile.pos.x + 1, tile.pos.y]);
        }
        if (tile.pos.x > 0)
        {
            CheckExpandRemoval(bb.tileMatrix[tile.pos.x - 1, tile.pos.y]);
        }
        if (tile.pos.y < 35)
        {
            CheckExpandRemoval(bb.tileMatrix[tile.pos.x, tile.pos.y + 1]);
        }
        if (tile.pos.y > 0)
        {
            CheckExpandRemoval(bb.tileMatrix[tile.pos.x, tile.pos.y - 1]);
        }
    }

    private void CheckExpandRemoval(Tile tile)
    {
        if (expandableTiles.ContainsKey(tile))
        {
            if (expandableTiles[tile] == 1)
            {
                expandableTiles.Remove(tile);
            }
            else
            {
                expandableTiles[tile]--;
            }
        }
    }

    public void ChoosePriorityTiles()
    {
        //Method that sorts tiles into higher or lower priority based on faction's preferences
        priorityTiles.Clear();
        List<Tile> warAndPreferred1 = new List<Tile>();
        List<Tile> warAndPreferred2 = new List<Tile>();
        List<Tile> warAndPreferred3 = new List<Tile>();
        List<Tile> war = new List<Tile>();
        List<Tile> newConquestPreferred1 = new List<Tile>();
        List<Tile> newConquestPreferred2 = new List<Tile>();
        List<Tile> newConquestPreferred3 = new List<Tile>();
        List<Tile> newConquest = new List<Tile>();
        List<Tile> preferred1 = new List<Tile>();
        List<Tile> preferred2 = new List<Tile>();
        List<Tile> preferred3 = new List<Tile>();
        List<Tile> betterMovement = new List<Tile>();
        List<Tile> otherMovement = new List<Tile>();
        List<Tile> lastResort = new List<Tile>();
        bool willingToDeclareWar = false;
        if (Random.value < warChance || Blackboard.totalTilesOwned >= 2304)
        {
            willingToDeclareWar = true;
        }
        //Slotting them into the different lists
        foreach (KeyValuePair<Tile, int> tile in expandableTiles)
        {
            if (!stillOwned.ContainsKey(tile.Key) || !stillOwned[tile.Key])
            {
                if (factionsAtWar.ContainsKey(tile.Key.owner))
                {
                    AddPreferredResources(warAndPreferred1, warAndPreferred2, warAndPreferred3, tile.Key, war);
                }
                else if (tile.Key.owner != -1 && willingToDeclareWar)
                {
                    AddPreferredResources(newConquestPreferred1, newConquestPreferred2, newConquestPreferred3, tile.Key, newConquest);
                }
                else if (tile.Key.owner == -1 && !AddPreferredResources(preferred1, preferred2, preferred3, tile.Key))
                {
                    if ((preferredMovement == MovementType.Land && tile.Key.land) || (preferredMovement == MovementType.Sea && !tile.Key.land))
                    {
                        betterMovement.Add(tile.Key);
                    }
                    else
                    {
                        otherMovement.Add(tile.Key);
                    }
                }
                else
                {
                    lastResort.Add(tile.Key);
                }
            }
        }
        //Putting the lists back together
        priorityTiles.AddRange(warAndPreferred1);
        priorityTiles.AddRange(warAndPreferred2);
        priorityTiles.AddRange(warAndPreferred3);
        priorityTiles.AddRange(war);
        priorityTiles.AddRange(newConquestPreferred1);
        priorityTiles.AddRange(newConquestPreferred2);
        priorityTiles.AddRange(newConquestPreferred3);
        priorityTiles.AddRange(newConquest);
        priorityTiles.AddRange(preferred1);
        priorityTiles.AddRange(preferred2);
        priorityTiles.AddRange(preferred3);
        priorityTiles.AddRange(betterMovement);
        priorityTiles.AddRange(otherMovement);
        priorityTiles.AddRange(lastResort);
        //Debug.Log(priorityTiles.Count);
        //Debug.Log(expandableTiles.Count);
    }

    private bool AddPreferredResources(List<Tile> list1, List<Tile> list2, List<Tile> list3, Tile tile, List<Tile> noneList = null)
    {
        //Helper method for the above method
        if (tile.resource == resourcePriorities[0])
        {
            list1.Add(tile);
            return true;
        }
        else if (tile.resource == resourcePriorities[1])
        {
            list2.Add(tile);
            return true;
        }
        else if (tile.resource == resourcePriorities[2])
        {
            list3.Add(tile);
            return true;
        }
        else if(noneList != null)
        {
            noneList.Add(tile);
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    public void Init(Blackboard bbIn)
    {
        //Inits the knowledge source with all of its preferences, and also changes what shows in the menu based on it
        bb = bbIn;
        tilesOwned = 0;
        factionsAtWar = new Dictionary<int, int>();
        priorityTiles = new List<Tile>();
        ownedTiles = new Dictionary<Tile, int>();
        expandableTiles = new Dictionary<Tile, int>();
        stillOwned = new Dictionary<Tile, bool>();
        food = 0;
        foodPerTurn = 2;
        money = 0;
        moneyPerTurn = 2;
        production = 0;
        productionPerTurn = 2;
        resourcePriorities = new Resource[3];
        int num1 = Random.Range(1, 4);
        int num2;
        while (true)
        {
            num2 = Random.Range(1, 4);
            if(num2 != num1)
            {
                break;
            }
        }
        resourcePriorities[0] = (Resource)num1;
        resourcePriorities[1] = (Resource)num2;
        switch (num1 + num2)
        {
            case 3:
                resourcePriorities[2] = (Resource)3;
                break;
            case 4:
                resourcePriorities[2] = (Resource)2;
                break;
            case 5:
                resourcePriorities[2] = (Resource)1;
                break;
            default:
                break;
        }
        warChance = Random.value;
        preferredMovement = (MovementType)Random.Range(0, 2);
        //String with info about the factions
        string factionString = "";
        Text text = GameObject.Find("Canvas").transform.GetChild(1).GetChild(id+1).GetComponent<Text>();
        switch (id)
        {
            case 0:
                color = Color.white;
                factionString += "White";
                break;
            case 1:
                color = Color.black;
                factionString += "Black";
                break;
            case 2:
                color = Color.gray;
                factionString += "Gray";
                break;
            case 3:
                color = new Color(1.0f, .647f, 0.0f);
                factionString += "Orange";
                break;
            case 4:
                color = new Color(.5f, 0.0f, .5f);
                factionString += "Purple";
                break;
            case 5:
                color = Color.cyan;
                factionString += "Cyan";
                break;
            default:
                break;
        }
        factionString += " faction, War Chance: " + (int)(warChance * 100.0f) + "%, Preferred Movement: ";
        if(preferredMovement == MovementType.Land)
        {
            factionString += "Land";
        }
        else
        {
            factionString += "Sea";
        }
        factionString += "\nPreferred Resources: ";
        for(int c = 0; c < 3; c++)
        {
            if(resourcePriorities[c] == Resource.Food)
            {
                factionString += "Food";
            }
            else if (resourcePriorities[c] == Resource.Money)
            {
                factionString += "Money";
            }
            else
            {
                factionString += "Production";
            }
            if(c != 2)
            {
                factionString += ", ";
            }
        }
        text.text = factionString;
    }
}
