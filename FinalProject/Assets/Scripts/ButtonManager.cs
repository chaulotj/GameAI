using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void ContinueGame()
    {
        ControlShell.autorun = false;
        GameObject.Find("Blackboard").GetComponent<ControlShell>().Run();
    }

    public void AutoRun()
    {
        ControlShell.autorun = true;
    }

    public void OpenFaction()
    {
        GameObject.Find("Canvas").transform.GetChild(1).GetComponent<CanvasDisplay>().SpawnFactionInfo(EventSystem.current.currentSelectedGameObject.GetComponent<HoldKnowledge>().k);
    }

}
