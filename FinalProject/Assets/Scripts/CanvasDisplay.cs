using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasDisplay : MonoBehaviour
{
    public GameObject button;
    public GameObject settlement;
    public Blackboard bb;
    private int y;
    // Start is called before the first frame update
    void Start()
    {
        bb = GameObject.Find("Blackboard").transform.GetComponent<Blackboard>();
        gameObject.SetActive(false);
    }

    public void SpawnButtons()
    {
        //Spawns buttons for each faction
        transform.GetChild(0).GetComponent<Image>().color = Color.black;
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);
        y = 230;
        foreach (Transform child in transform.GetChild(1))
        {
            Destroy(child.gameObject);
        }
        foreach (KeyValuePair<int, KnowledgeSource> k in bb.factions)
        {
            GameObject newButton = Instantiate(button, new Vector3(transform.position.x, transform.position.y + y, transform.position.z), Quaternion.identity, transform.GetChild(1));
            newButton.GetComponent<Image>().color = k.Value.color;
            newButton.transform.GetChild(0).GetComponent<Text>().color = k.Value.oppositeColor;
            newButton.transform.GetChild(0).GetComponent<Text>().text = k.Value.name;
            newButton.GetComponent<HoldKnowledge>().k = k.Value;
            y -= 50;
        }
    }

    public void SpawnFactionInfo(KnowledgeSource k)
    {
        //shows info about a certain faction
        transform.GetChild(0).GetComponent<Image>().color = k.color;
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
        Transform display = transform.GetChild(2);
        for(int c = 0; c < 12; c++)
        {
            display.GetChild(c).GetComponent<Text>().color = k.oppositeColor;
        }
        display.GetChild(0).GetComponent<Text>().text = k.name + ".";
        display.GetChild(1).GetComponent<Text>().text = "Leader: " + k.rulerName;
        display.GetChild(2).GetComponent<Text>().text = "Rebellion Chance: " + k.rulerUnpopularity.ToString("F3") + "%";
        display.GetChild(3).GetComponent<Text>().text = "War Chance: " + (int)(k.warChance * 100f) + "%";
        string movementStr = "Preferred Movement: ";
        if (k.preferredMovement == MovementType.Land)
        {
            movementStr += "Land";
        }
        else
        {
            movementStr += "Sea";
        }
        display.GetChild(4).GetComponent<Text>().text = movementStr;
        string preferredStr =  "Resource Priorities: ";
        for (int c = 0; c < 3; c++)
        {
            if (k.resourcePriorities[c] == Resource.Food)
            {
                preferredStr += "Food";
            }
            else if (k.resourcePriorities[c] == Resource.Money)
            {
                preferredStr += "Money";
            }
            else
            {
                preferredStr += "Production";
            }
            if (c != 2)
            {
                preferredStr += ", ";
            }
        }
        display.GetChild(5).GetComponent<Text>().text = preferredStr;
        display.GetChild(6).GetComponent<Text>().text = "Death Chance: " + k.rulerPoorHealth.ToString("F3") + "%";
        display.GetChild(7).GetComponent<Text>().text = "Leader War Skill: " + k.rulerWarSkill;
        display.GetChild(8).GetComponent<Text>().text = "Leader Food Skill: " + k.rulerFoodSkill;
        display.GetChild(9).GetComponent<Text>().text = "Leader Money Skill: " + k.rulerMoneySkill;
        display.GetChild(10).GetComponent<Text>().text = "Leader Production Skill: " + k.rulerProductionSkill;
        Transform settlements = display.GetChild(11);
        foreach (Transform child in settlements)
        {
            Destroy(child.gameObject);
        }
        int x = -450;
        int y = 100;
        int d = 0;
        foreach(KeyValuePair<string, Tile> s in k.settlements)
        {
            Text set = Instantiate(settlement, new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z), Quaternion.identity, settlements).GetComponent<Text>();
            set.color = k.oppositeColor;
            set.text = s.Key + ", Level " + (s.Value.settlementLevel + 1);
            d++;
            if(d % 4 == 0)
            {
                d = 0;
                x = -450;
                y -= 45;
            }
            else
            {
                x += 225;
            }
        }
    }
}
