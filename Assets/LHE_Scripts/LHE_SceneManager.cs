using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LHE_SceneManager : MonoBehaviour
{
    // (1) 승리
    // 인게임에서 승리 Image 일정시간동안 보여줌
    // 일정시간 이후 승리 Scene Load

    // (2) 패배
    // 인게임에서 패배 Image 일정시간동안 보여줌
    // 일정시간 이후 패배 Scene Load

    public GameObject defeat; // 패배 Letter Image GO
    public GameObject victory; // 승리 Letter Image GO

    GameObject player;
    GameObject enemyNear;
    GameObject enemyFar;

    // Start is called before the first frame update
    void Start()
    {
        victory.SetActive(false);
        defeat.SetActive(false);

        player = GameObject.Find("Player");
        enemyNear = GameObject.Find("EnemyNear");
        enemyFar = GameObject.Find("EnemyFar");
    }

    // Update is called once per frame
    void Update()
    {
        // (1) 승리
        if(player && !enemyNear && !enemyFar)
        {
            StartCoroutine("IeVictory");
        }

        // (2) 패배
        if(!player && ( enemyNear || enemyFar))
        {
            StartCoroutine("IeDefeat");
        }
    }
    IEnumerator IeVictory()
    {
        victory.SetActive(true);
        yield return new WaitForSeconds(10f);
        //SceneManager.LoadScene("LHE_AfterGameUI_Victory");
        SceneManager.LoadScene("LHE_MorusIsland");
    }


    IEnumerator IeDefeat()
    {
        defeat.SetActive(true);
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("LHE_AfterGameUI_Defeat");
    }
}
