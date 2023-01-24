using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LHE_MapButtonLoadScene : MonoBehaviour
{
    // Map UI에서 "광장 입장" 버튼 클릭 시 해당 스크립트의 하기 함수 호출해 씬 전환
    public void LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
