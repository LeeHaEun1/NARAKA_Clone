using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_WallTrigger : MonoBehaviour
{
    public static LHE_WallTrigger Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public enum State
    {
        PlayerEnterWall,
        PlayerExitWall
    }
    public State state;

    // Start is called before the first frame update
    void Start()
    {
        state = State.PlayerExitWall;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("player enter wall");
            state = State.PlayerEnterWall;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("player enter wall");
            state = State.PlayerEnterWall;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            print("player exit wall");
            state = State.PlayerExitWall;
            StartCoroutine("IeEnemyJump");
            //LHE_EnemyMove.Instance.state = LHE_EnemyMove.State.Jump;
        }
    }

    //IEnumerator IeEnemyJump()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
