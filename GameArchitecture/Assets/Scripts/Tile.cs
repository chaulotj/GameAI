using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    None
}

public class Tile : MonoBehaviour
{
    public bool land; //If not land, then water
    public Resource resource;
    public int owner; //0 is no one
    // Start is called before the first frame update
    void Start()
    {
        if (land)
        {
            GetComponent<MeshRenderer>().material.color = Color.yellow;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
