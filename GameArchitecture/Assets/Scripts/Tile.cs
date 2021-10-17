using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
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
    public int owner; //0 is no one
    public Vector2Int pos;
    // Start is called before the first frame update
    void Start()
    {
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
}
