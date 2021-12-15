using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    public Tile[,] tileMatrix;
    public List<Tile> tileList;
    public Dictionary<int, KnowledgeSource> factions;
    public int factionCount;
    public static int totalFactions;
    public List<Tile> landTileList;
    public static int totalTilesOwned;
    public static List<string> empireTitles;
    public static List<string> empireTitles2;
    public static List<string> settlementNames;
    public static List<string> empireNames;
    public static List<string> characterFirstNames;
    public static List<string> characterLastNames;

    // Start is called before the first frame update
    void Awake()
    {
        factionCount = 6;
        totalFactions = 6;
        //All possible names
        settlementNames = new List<string>()
        {
            "Faiyum", "Annaba", "Luxor", "Kismayo", "Zeila", "Aksum", "Oujda", "Kano", "Ouidah", "Sofala", "Cholula", "Tucson",
            "Cartago", "Hampton", "Hopewell", "Flores", "San Juan", "Baracoa", "Toronto", "Winnipeg", "Quito", "Cusco", "Cali", "Piura",
            "Asuncion", "Cumana", "Santiago", "Salvador", "Marta", "Lima", "Multan", "Rajgir", "Ujjain", "Quetta", "Nara", "Gyeongju",
            "Weinan", "Suzhou", "Miyako", "Pyay", "Lamphun", "Butuan", "Kediri", "Byblos", "Homs", "Yazd", "Medina", "Hobart",
            "Chalcis", "Ravenna", "Cadiz", "Byzantion", "Marseille", "Nesebar", "Qabala", "Trier", "Nis", "Derbent", "Lofoten", "Uppsala",
            "Yew Nork"
        };
        empireTitles = new List<string>()
        {
            "People's Democratic Republic of ", "Empire of ", "Kingdom of ", "City-state of ", "Holy Order of ", "Legion of ", "Glorious Empire of ", "Shadowy Conglomerate of ",
            "Union of ", "Democratic Union of ", "Holy Kingdom of ", "Holy Empire of ", "Order of ", "United States of ", "Federation of ", "Cabal of "
        };
        empireTitles2 = new List<string>()
        {
            "Republican ", "Oligarchical ", "Corporate ", "Democratic ", "Dictatorial ", "Monarchical ", "Colonial ", "Aristocratic ", "Noble ", "Theocratic ",
            "Fascist ", "Communist ", "Anarchist ", "Capitalist ", "Socialist ", "Libertarian ", "Militaristic ", "Totalitarian ", "Tubular ", "Spiritualistic ", "Pacifist "
        };
        empireNames = new List<string>()
        {
            "Gamers", "Crimea", "Armenia", "Texas", "Serbia", "Singapore", "Vatican", "Monaco", "Luxembourg", "Metaverse", "Gotham", "Westeros", "Skyrim", "Morthal", "Arrakis", "Caladan",
            "Carthage", "Seleucids", "New Vegas", "New California", "Essos", "Togo", "Ethiopia", "Rome", "Reach", "Lordran", "Lothric", "Hokkaido", "Siberia", "Uruguay", "Nicaragua", "Chile",
            "Canada", "Tajikistan", "Milan", "Sicily", "Malaysia", "Egypt", "Greece", "Venice", "Norway", "Denmark", "Sweden", "Tahiti", "Nilfgaard", "Temeria", "Redania", "Kaedwen", "Aedirn",
            "Toussaint", "Kovir", "Poviss", "India", "Mexico", "Dimmesdale", "Germany", "Russia", "America", "Amazon", "Fremen", "Aztec"
        };
        characterFirstNames = new List<string>()
        {
            "Nazeem", "Ulfric", "Doug", "Joseph", "Frank", "Jonathan", "Jotaro", "Josuke", "Giorno", "Jolyne", "Sweeney", "Todd", "Howard", "Elizabeth", "Anne", "Mary", "Fred", "Kira", "Sun", "Mulan",
            "Deez", "Jesus", "Shaka", "Henrietta", "Eko", "Trish", "Joanna", "Joan", "Eva", "Anastasia", "Cherlene", "Aoi", "Arya", "Pacman", "Pacwoman", "Pacperson", "Chani", "Rango", "Katya", "Lana",
            "Pamela", "Mallory", "Sterling", "Hunter", "Eren", "Mikasa", "Vladimir", "Ragnar", "Kratos", "Atreus", "Laufey", "Freya", "Confucius", "Mohammed", "Ali", "Atimah", "Lars", "Bender", "Hubert",
            "Philip", "Turanga", "Aang", "Sokka", "Katara", "Toph", "Zuko", "Azula", "Barry", "Mance", "Spongebob", "Squidward", "Spencer", "Chewbacca", "Lightning"
        };
        characterLastNames = new List<string>()
        {
            "Joestar", "Brando", "Dimmadome", "Stormcloak", "Kujo", "Giovanna", "Sweeney", "Todd", "Howard", "Nietzche", "Marx", "Engles", "Miyazaki", "Tzu", "Yoshikage", "Beesworth", "Nuts", "Christ",
            "Zulu", "Archer", "Krieger", "Jaeger", "Buddha", "Mohammed", "Ali", "Bjornsdottir", "Ragnarsson", "McDonald", "McClary", "Larsson", "Fry", "Leela", "Rodriguez", "Zoidberg", "Farnsworth",
            "Squarepants", "Star", "Tentacles", "Krabs", "Cheeks", "Capet", "Villeneuve", "Craster", "Rayder", "Napoleon", "Grievous", "Kenobi", "Skywalker", "Solo", "McQueen", "Atreides", "Harkonnen",
            "Bolt"
        };
        //All my data structures
        tileMatrix = new Tile[64, 36];
        tileList = new List<Tile>();
        factions = new Dictionary<int, KnowledgeSource>();
        landTileList = new List<Tile>();
        totalTilesOwned = 0;
        for(int c = 0; c < factionCount; c++)
        {
            //Inits all knowledge sources
            KnowledgeSource k = new KnowledgeSource();
            k.id = c;
            k.Init(this);
            factions.Add(c, k);
        }
        for (int c = 0; c < transform.childCount; c++)
        {
            //Fills all tile structures
            Transform row = transform.GetChild(c);
            for (int d = 0; d < row.childCount; d++)
            {
                Tile tile = row.GetChild(d).GetComponent<Tile>();
                tile.pos = new Vector2Int(d, c);
                tileMatrix[d, c] = tile;
                tileList.Add(tile);
                if (tile.land)
                {
                    landTileList.Add(tile);
                }
            }
        }
        foreach (Tile t in tileList)
        {
            //Generates the world's tiles
            t.owner = -1;
            if (t.resource == Resource.None)
            {
                float f = Random.value;
                if (f < .008f)
                {
                    t.resource = Resource.Food;
                    MakeMoreResource(t, Resource.Food, .5f);
                }
                else if (f < .018f)
                {
                    t.resource = Resource.Money;
                    MakeMoreResource(t, Resource.Money, .2f);
                }
                else if (f < .033f)
                {
                    t.resource = Resource.Production;
                    MakeMoreResource(t, Resource.Production, .35f);
                }
            }
        }
    }
    
    void MakeMoreResource(Tile tile, Resource resource, float chance)
    {
        //Recursive function to make resources appear in clumps
        if (tile.pos.x < 63 && tileMatrix[tile.pos.x + 1, tile.pos.y].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x + 1, tile.pos.y].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x + 1, tile.pos.y], resource, chance * .25f);
        }
        if (tile.pos.x > 0 && tileMatrix[tile.pos.x - 1, tile.pos.y].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x - 1, tile.pos.y].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x - 1, tile.pos.y], resource, chance * .25f);
        }
        if (tile.pos.y < 35 && tileMatrix[tile.pos.x, tile.pos.y + 1].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x, tile.pos.y + 1].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x, tile.pos.y + 1], resource, chance * .25f);
        }
        if (tile.pos.y > 0 && tileMatrix[tile.pos.x, tile.pos.y - 1].resource == Resource.None && Random.value < chance)
        {
            tileMatrix[tile.pos.x, tile.pos.y - 1].resource = resource;
            MakeMoreResource(tileMatrix[tile.pos.x, tile.pos.y - 1], resource, chance * .25f);
        }
    }
}
