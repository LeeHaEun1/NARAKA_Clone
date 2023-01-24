using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_SurviveTime : MonoBehaviour
{
    Text surviveUI;

    float survivalTime;

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        survivalTime = LHE_SurvivalTimeRecord.Instance.survivalTime;
        surviveUI = GetComponent<Text>();
        surviveUI.text = (int)survivalTime / 60 + " : " + Mathf.Round(survivalTime % 60);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
