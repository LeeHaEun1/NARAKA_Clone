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
        // �й� Scene���� ����ϱ� ����
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
        // �����ð� üũ�� ���� �÷��̾ ������ Ÿ�̸� ����
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
