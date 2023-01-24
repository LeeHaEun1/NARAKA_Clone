using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LHE_SceneManager : MonoBehaviour
{
    // (1) �¸�
    // �ΰ��ӿ��� �¸� Image �����ð����� ������
    // �����ð� ���� �¸� Scene Load

    // (2) �й�
    // �ΰ��ӿ��� �й� Image �����ð����� ������
    // �����ð� ���� �й� Scene Load

    public GameObject defeat; // �й� Letter Image GO
    public GameObject victory; // �¸� Letter Image GO

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
        // (1) �¸�
        if(player && !enemyNear && !enemyFar)
        {
            StartCoroutine("IeVictory");
        }

        // (2) �й�
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
