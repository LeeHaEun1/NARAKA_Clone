using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LHE_Timer : MonoBehaviour
{
    public static LHE_Timer Instance;

    Text timerUI;

    float currentTime = 0;
    public float limitTime = 1800;
    public float timeMultiplier = 50;

    GameObject player;

    private void Awake()
    {
        // 패배 Scene에서 사용하기 위해
        //DontDestroyOnLoad(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        timerUI = GetComponent<Text>();
        timerUI.text = (int)limitTime / 60 + " : " + Mathf.Round(limitTime % 60);

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // 생존시간 체크를 위해 플레이어가 죽으면 타이머 정지
        if (player)
        {
            currentTime += Time.fixedDeltaTime;
            if (currentTime > Time.fixedDeltaTime * timeMultiplier)
            {
                limitTime--;
                timerUI.text = (int)limitTime / 60 + " : " + Mathf.Round(limitTime % 60);
                currentTime = 0;
            }
        }
    }
}
