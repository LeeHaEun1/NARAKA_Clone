using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_Kill : MonoBehaviour
{
    Text killUI;

    int kill;

    // Start is called before the first frame update
    void Start()
    {
        kill = LHE_SurvivalTimeRecord.Instance.kill;
        killUI = GetComponent<Text>();
        killUI.text = kill + ""; // string 변환 위해 ""
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
