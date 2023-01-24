using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class YSMDashGage : MonoBehaviour
{
    public static YSMDashGage Instance = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    [SerializeField]
    public Image imageCooldown;
    //[SerializeField]
    //TMP_Text textCooldown;

    bool isColldown = false;
    float cooldownTime = 2f;
    public float cooldownTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //textCooldown.gameObject.SetActive(false);
        imageCooldown.fillAmount = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.Q))
        {
            UseSpell();
        }*/
        if (isColldown)
        {
            ApplyCooldown();
        }
    }

    void ApplyCooldown()
    {
        cooldownTimer += Time.deltaTime / 3 * 2;

        if (cooldownTimer > 2.0f)
        {
            imageCooldown.enabled = false;

            isColldown = false;
            //textCooldown.gameObject.SetActive(false);
            imageCooldown.fillAmount = 1.0f;
            cooldownTimer = 2.0f;

        }
        else if(cooldownTimer < 0.0f)
        {
            imageCooldown.fillAmount = 0.0f;

            cooldownTimer = 0.0f;

        }
        else
        {
            imageCooldown.enabled = true;

            //textCooldown.text = Mathf.RoundToInt(cooldownTimer).ToString();
            imageCooldown.fillAmount = cooldownTimer / cooldownTime;
        }
    }


    public void UseSpell()
    {
        if (isColldown)
        {

        }
        else
        {
            isColldown = true;
            //textCooldown.gameObject.SetActive(true);
            cooldownTimer = cooldownTime;
        }
    }
}
