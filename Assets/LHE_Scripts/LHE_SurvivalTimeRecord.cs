using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (1)생존시간과 (2)처치 수를 저장하는 별도의 게임오브젝트 생성
public class LHE_SurvivalTimeRecord : MonoBehaviour
{
    public static LHE_SurvivalTimeRecord Instance;

    public float survivalTime;
    public int kill;

    GameObject player;
    GameObject enemyNear;
    GameObject enemyFar;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        enemyNear = GameObject.Find("EnemyNear");
        enemyFar = GameObject.Find("EnemyFar");
    }

    // Update is called once per frame
    void Update()
    {
        // (1) 생존 시간
        if (!player)
        {
            survivalTime = 1800 - LHE_Timer.Instance.limitTime;
        }

        // (2) 처치 수
        if(enemyNear && enemyFar)
        {
            kill = 0;
        }
        else
        {
            kill = 1;
        }
    }
}
