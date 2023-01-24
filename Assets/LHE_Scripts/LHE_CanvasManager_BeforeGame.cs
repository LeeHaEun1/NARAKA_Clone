using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class LHE_CanvasManager_BeforeGame : MonoBehaviour
{
    public CanvasGroup entertainmentGroup;
    public CanvasGroup warningGroup;
    public CanvasGroup titleGroup;
    public CanvasGroup cinematicGroup; // Video
    public CanvasGroup characterGroup; // Video
    public CanvasGroup loadingGroup;
    public CanvasGroup mapGroup;

    public Canvas entertainment;
    public Canvas warning;
    public Canvas title;
    public Canvas cinematic; // Video
    public Canvas character; // Video
    public Canvas loading; 
    public Canvas map;

    AudioSource warningAudio;
    AudioSource characterAudio;
    AudioSource mapAudio;

    public VideoPlayer cinematicVideo;
    public VideoPlayer characterVideo;

    public Text mapTimerText;

    // FSM
    public enum State
    {
        Entertainment,
        Warning,
        Title,
        Cinematic,
        Character,
        Loading,
        Map
    }
    public State state;

    // Time Management
    public float currentTime;

    // Start is called before the first frame update
    void Start()
    {
        // ��� CanvasGroup ��Ȱ��ȭ�ϰ� ����
        entertainmentGroup.enabled = false;
        warningGroup.enabled = false;
        titleGroup.enabled = false;
        cinematicGroup.enabled = false;
        characterGroup.enabled = false;
        loadingGroup.enabled = false;
        mapGroup.enabled = false;

        // ��� Canvas ��Ȱ��ȭ�ϰ� ����
        entertainment.enabled = false;
        warning.enabled = false;
        title.enabled = false;
        cinematic.enabled = false;
        character.enabled = false;
        loading.enabled = false;
        map.enabled = false;

        // ���� ����
        state = State.Entertainment;

        // AudioSource
        warningAudio = warning.gameObject.GetComponent<AudioSource>();
        characterAudio = character.gameObject.GetComponent<AudioSource>();
        mapAudio = map.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // FSM
        switch (state)
        {
            case State.Entertainment:
                StateEntertainment();
                break;
            case State.Warning:
                StateWarning();
                break;
            case State.Title:
                StateTitle();
                break;
            case State.Cinematic:
                StateCinematic();
                break;
            case State.Character:
                StateCharacter();
                break;
            case State.Loading:
                StateLoading();
                break;
            case State.Map:
                StateMap();
                break;
        }
    }

    public float entertainmentTime = 10;
    private void StateEntertainment()
    {
        entertainmentGroup.enabled = true;
        entertainment.enabled = true;

        currentTime += Time.fixedDeltaTime;
        if(currentTime > entertainmentTime)
        {
            currentTime = 0;

            entertainmentGroup.enabled = false;
            entertainment.enabled = false;

            warningGroup.enabled = true;
            warning.enabled = true;
            warningAudio.Play();

            state = State.Warning;
        }
    }

    public float warningTime = 15;
    private void StateWarning()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > warningTime)
        {
            currentTime = 0;

            warningGroup.enabled = false;
            warning.enabled = false;

            titleGroup.enabled = true;
            title.enabled = true;

            state = State.Title;
        }
    }

    private void StateTitle()
    {
        // ��ư Ŭ�� ��
        if(cinematicGroup.enabled == true)
        {
            titleGroup.enabled = false;
            title.enabled = false;

            warningAudio.mute = true;

            cinematic.enabled = true;

            cinematicVideo.Play(); // ���� ���

            state = State.Cinematic;
        }
    }

    private void StateCinematic()
    {
        // (1) Skip �ϴ� ���
        // EscŰ ������ �����׿��� Skip
        if (Input.GetKey(KeyCode.Escape))
        {
            cinematicVideo.Pause();

            cinematicGroup.enabled = false;
            cinematic.enabled = false;

            characterGroup.enabled = true;
            character.enabled = true;

            characterVideo.Play(); // ���� ���
            characterAudio.Play(); // ����� ���

            state = State.Character;
        }

        // (2) Skip ���� �ʴ� ���
        // ������ ����ð��밡 ���� ���̿� ��ġ�Ѵٸ�(=����� �����ٸ�) ���� ���·� ��ȯ
        // print��� time(91.93..)�� length(91.96..)�� ������ ���ؼ� == ����ϸ� �ǵ���� ���� X, ���� �ε�ȣ�� ����
        if (cinematicVideo.time > cinematicVideo.clip.length - 0.1f)
        {
            cinematicGroup.enabled = false;
            cinematic.enabled = false;

            characterGroup.enabled = true;
            character.enabled = true;

            characterVideo.Play(); // ���� ���
            characterAudio.Play(); // ����� ���

            state = State.Character;
        }
        print("cinematicVideo.clip.length " + cinematicVideo.clip.length);
        print("cinematicVideo.time " + cinematicVideo.time);
    }

    private void StateCharacter()
    {
        // ��ư Ŭ�� ��
        if (loadingGroup.enabled == true)
        {
            characterGroup.enabled = false;
            character.enabled = false;

            characterAudio.mute = true;

            loading.enabled = true;

            state = State.Loading;
        }
    }

    public float loadingTime = 20;
    private void StateLoading()
    {
        currentTime += Time.fixedDeltaTime;
        if (currentTime > loadingTime)
        {
            currentTime = 0;

            loadingGroup.enabled = false;
            loading.enabled = false;

            mapGroup.enabled = true;
            map.enabled = true;

            mapAudio.Play(); // ����� ���

            state = State.Map;
        }
    }

    public float mapChooseTime = 30;
    public float timeMultiplier = 50;
    private void StateMap()
    {
        // [Timer ����]
        // (1) Ÿ�̸� ���ѽð� �����Ǹ� �ڵ����� �� ��ȯ
        mapTimerText.text = (int)mapChooseTime + ""; // string ��ȯ ���� "" ������
        currentTime += Time.fixedDeltaTime;
        if (currentTime > Time.fixedDeltaTime * timeMultiplier)
        {
            mapChooseTime--;
            mapTimerText.text = (int)mapChooseTime + ""; // string ��ȯ ���� "" ������
            currentTime = 0;
        }
        if(mapChooseTime < 0.1f)
        {
            mapAudio.mute = true; // ����� mute
            SceneManager.LoadScene("SampleScene");
        }

        // (2) ������ �����ϰ� "���� ����" ��ư ������ ��� -> ��ư ������Ʈ���� ����?
    }
}
