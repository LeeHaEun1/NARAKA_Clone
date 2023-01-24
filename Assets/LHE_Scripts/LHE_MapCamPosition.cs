using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_MapCamPosition : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z; // �� ���� true�̸� player�� ��ǥ, false�̸� ���� ��ǥ�� ���
    
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // player ������ ����
        if (!player) return;

        transform.position = new Vector3(
            (x ? player.transform.position.x : transform.position.x),
            (y ? player.transform.position.y : transform.position.y),
            (z ? player.transform.position.z : transform.position.z));
    }
}
