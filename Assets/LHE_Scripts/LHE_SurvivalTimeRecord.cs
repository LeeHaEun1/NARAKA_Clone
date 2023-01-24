using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// (1)�����ð��� (2)óġ ���� �����ϴ� ������ ���ӿ�����Ʈ ����
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
        // (1) ���� �ð�
        if (!player)
        {
            survivalTime = 1800 - LHE_Timer.Instance.limitTime;
        }

        // (2) óġ ��
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
