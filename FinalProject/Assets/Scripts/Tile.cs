using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource //It takes 2 food to move across land, and 1 food and 1 production to move on the sea. Attacking an enemy tile (beyond the normal costs) takes an extra 2 production and 2 money. Taking a resource costs 1 production
{
    None,
    Food,
    Money,
    Production
}

public class Tile : MonoBehaviour
{
    public bool land; //If not land, then water
    public Resource resource; //Most will be none
    public int owner; //-1 is no one
    public Vector2Int pos;
    public int settlementLevel;
    public string settlementName;
    // Start is called before the first frame update
    void Start()
    {
        settlementLevel = -1;
        //Just choosing colors
        if(resource == Resource.Food)
        {
            GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else if(resource == Resource.Money)
        {
            GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
        else if (resource == Resource.Production)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else if (land)
        {
            GetComponent<MeshRenderer>().material.color = new Color(.408f, .255f, .196f);
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }

    public void IncreaseSettlementLevel()
    {
        settlementLevel++;
        transform.GetChild(1).GetChild(settlementLevel).gameObject.SetActive(true);
        if(settlementLevel == 0)
        {
            int index = Random.Range(0, Blackboard.settlementNames.Count);
            settlementName = Blackboard.settlementNames[index];
            Blackboard.settlementNames.RemoveAt(index);
        }
    }

    public void DecreaseSettlementLevel()
    {
        settlementLevel -= 3;
        if(settlementLevel < 0)
        {
            settlementLevel = 0;
        }
        for(int c = settlementLevel + 1; c < settlementLevel + 4; c++)
        {
            transform.GetChild(1).GetChild(c).gameObject.SetActive(false);
        }
    }
}
