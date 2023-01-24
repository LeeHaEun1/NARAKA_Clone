using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_ProfileBGColor : MonoBehaviour
{
    Image profileBG;
    int playerHP;

    // Start is called before the first frame update
    void Start()
    {
        profileBG = GetComponent<Image>();
        playerHP = LHE_test_PlayerHP.Instance.HP;
    }

    // Update is called once per frame
    void Update()
    {
        if(LHE_test_PlayerHP.Instance.HP != playerHP)
        {
            playerHP = LHE_test_PlayerHP.Instance.HP;
            StartCoroutine("IeRedBG");       
        }
    }

    IEnumerator IeRedBG()
    {
        profileBG.color = new Color(0.8f, 0, 0, 0.471f);
        yield return new WaitForSeconds(0.4f);
        profileBG.color = new Color(0, 0, 0, 0.471f);
    }
}
