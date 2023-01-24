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
        // 모든 CanvasGroup 비활성화하고 시작
        entertainmentGroup.enabled = false;
        warningGroup.enabled = false;
        titleGroup.enabled = false;
        cinematicGroup.enabled = false;
        characterGroup.enabled = false;
        loadingGroup.enabled = false;
        mapGroup.enabled = false;

        // 모든 Canvas 비활성화하고 시작
        entertainment.enabled = false;
        warning.enabled = false;
        title.enabled = false;
        cinematic.enabled = false;
        character.enabled = false;
        loading.enabled = false;
        map.enabled = false;

        // 시작 상태
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
        // 버튼 클릭 시
        if(cinematicGroup.enabled == true)
        {
            titleGroup.enabled = false;
            title.enabled = false;

            warningAudio.mute = true;

            cinematic.enabled = true;

            cinematicVideo.Play(); // 영상 재생

            state = State.Cinematic;
        }
    }

    private void StateCinematic()
    {
        // (1) Skip 하는 경우
        // Esc키 누르면 오프닝영상 Skip
        if (Input.GetKey(KeyCode.Escape))
        {
            cinematicVideo.Pause();

            cinematicGroup.enabled = false;
            cinematic.enabled = false;

            characterGroup.enabled = true;
            character.enabled = true;

            characterVideo.Play(); // 영상 재생
            characterAudio.Play(); // 배경음 재생

            state = State.Character;
        }

        // (2) Skip 하지 않는 경우
        // 영상의 재생시간대가 영상 길이와 일치한다면(=재생이 끝났다면) 다음 상태로 전환
        // print결과 time(91.93..)이 length(91.96..)에 도달을 못해서 == 사용하면 의도대로 실행 X, 따라서 부등호로 변경
        if (cinematicVideo.time > cinematicVideo.clip.length - 0.1f)
        {
            cinematicGroup.enabled = false;
            cinematic.enabled = false;

            characterGroup.enabled = true;
            character.enabled = true;

            characterVideo.Play(); // 영상 재생
            characterAudio.Play(); // 배경음 재생

            state = State.Character;
        }
        print("cinematicVideo.clip.length " + cinematicVideo.clip.length);
        print("cinematicVideo.time " + cinematicVideo.time);
    }

    private void StateCharacter()
    {
        // 버튼 클릭 시
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

            mapAudio.Play(); // 배경음 재생

            state = State.Map;
        }
    }

    public float mapChooseTime = 30;
    public float timeMultiplier = 50;
    private void StateMap()
    {
        // [Timer 진행]
        // (1) 타이머 제한시간 소진되면 자동으로 씬 전환
        mapTimerText.text = (int)mapChooseTime + ""; // string 변환 위해 "" 더해줌
        currentTime += Time.fixedDeltaTime;
        if (currentTime > Time.fixedDeltaTime * timeMultiplier)
        {
            mapChooseTime--;
            mapTimerText.text = (int)mapChooseTime + ""; // string 변환 위해 "" 더해줌
            currentTime = 0;
        }
        if(mapChooseTime < 0.1f)
        {
            mapAudio.mute = true; // 배경음 mute
            SceneManager.LoadScene("SampleScene");
        }

        // (2) 시작점 선택하고 "광장 입장" 버튼 누르는 경우 -> 버튼 컴포넌트에서 구현?
    }
}
