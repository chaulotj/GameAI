using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
