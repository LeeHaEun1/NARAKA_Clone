using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LHE_MapButtonLoadScene : MonoBehaviour
{
    // Map UI���� "���� ����" ��ư Ŭ�� �� �ش� ��ũ��Ʈ�� �ϱ� �Լ� ȣ���� �� ��ȯ
    public void LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
