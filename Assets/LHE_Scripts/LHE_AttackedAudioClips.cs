using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LHE_AttackedAudioClips : MonoBehaviour
{
    public static LHE_AttackedAudioClips Instance;
    public List<AudioClip> attackedAudioClips;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
