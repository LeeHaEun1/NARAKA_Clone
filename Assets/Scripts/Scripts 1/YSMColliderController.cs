using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YSMColliderController : MonoBehaviour
{
    BoxCollider swordCollider;

    public GameObject swordAttackImpactFactroy;
    public Transform sword;
    private AudioSource swordSound;


    // Start is called before the first frame update
    void Start()
    {
        swordCollider = GameObject.FindWithTag("Sword").GetComponent<BoxCollider>();

        swordSound = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ColliderControll()
    {
        print("ColliderControll");
        StopCoroutine(IE_ColliderControll());
        StartCoroutine(IE_ColliderControll());
    }
    public void ColliderControllFalse()
    {
        swordCollider.enabled = false;
    }

    IEnumerator IE_ColliderControll()
    {
        swordCollider.enabled = true;

        yield return new WaitForSeconds(0.6f);

        swordCollider.enabled = false;
    }

    public void SwordAttackImpact()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy);
        //swordAttackImpact.transform.forward = sword.transform.up;
        //swordAttackImpact.transform.position = transform.position;
        swordAttackImpact.transform.parent = transform.parent;
        swordAttackImpact.transform.localPosition = new Vector3 (0,1,0);
       // swordAttackImpact.transform.forward = transform.forward;
    }

    public GameObject swordAttackImpactFactroy1;

    public void SwordAttackImpact1()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy1);
        //swordAttackImpact.transform.forward = transform.forward;
        swordAttackImpact.transform.position = transform.position + transform.up * 3;
    }

    public GameObject swordAttackImpactFactroy2;

    public void SwordAttackImpact2()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy2);
        swordAttackImpact.transform.position = transform.position + transform.up*2;
    }

    public GameObject swordAttackImpactFactroy3;

    public void SwordAttackImpact3()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy3);
        swordAttackImpact.transform.position = transform.position + transform.up * 2 + transform.forward;
    }

    public GameObject swordAttackImpactFactroy4;

    public void SwordAttackImpact4()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy4);
        swordAttackImpact.transform.position = transform.position + transform.up * 2;
    }

    public GameObject swordAttackImpactFactroy5;
    public void SwordAttackImpact5()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy5);
        swordAttackImpact.transform.position = transform.position + transform.up * 3;
        swordAttackImpact.transform.forward = Camera.main.transform.right;
        /*swordAttackImpact.transform.parent = transform.parent;
        swordAttackImpact.transform.localPosition = Vector3.zero;*/
    }

    public GameObject swordAttackImpactFactroy6;
    public void SwordAttackImpact6()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy6);
        swordAttackImpact.transform.position = transform.position + transform.up * 4;
        swordAttackImpact.transform.forward = -Camera.main.transform.right;

    }

    public GameObject swordAttackImpactFactroy7;
    public void SwordAttackImpact7()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy7);
        swordAttackImpact.transform.position = transform.position + transform.up;
        swordAttackImpact.transform.forward = -transform.right;

    }

    public GameObject swordAttackImpactFactroy8;
    public void SwordAttackImpact8()
    {
        GameObject swordAttackImpact = Instantiate(swordAttackImpactFactroy8);
        swordAttackImpact.transform.position = transform.position + transform.up;
    }

    public GameObject swordAttackImpactFactroy9;
    public void SwordAttackImpact9()
    {
        GameObject attackImpact = Instantiate(swordAttackImpactFactroy9);
        attackImpact.transform.position = transform.position + transform.up * 2;
        attackImpact.transform.forward = Camera.main.transform.forward;
    }

    public void swordAudio()
    {
        swordSound.Stop();
        swordSound.Play();
    }

}
