using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    void Awake()
    {
        if(!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    [SerializeField] private AudioSource[] bgm;
    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] specialSFX;
    private int bgmIndex;

    public void PlaySFX(int sfxToPlay, bool interupt = false)
    {
        if(sfxToPlay >= sfx.Length)
            return;

        if(interupt)
            for(int i = 0; i < sfx.Length; i++)
                StopSFX(i);

        sfx[sfxToPlay].Play();
    }

    public void PlayBGM(int bgmToPlay = -1, bool loop = false)
    {
        bgmIndex = bgmToPlay == -1 ? UnityEngine.Random.Range(0, bgm.Length) : bgmToPlay;

        for(int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
            bgm[i].loop = false;
        }

        bgm[bgmIndex].loop = loop;
        bgm[bgmIndex].Play();
    }

    public void PlaySpecialSFX(int specialSFX)
    {
        for(int i = 0; i < sfx.Length; i++)
            StopSFX(i);

        this.specialSFX[specialSFX].Play();
    }

    public bool isPlayingSFX()
    {
        for(int i = 0; i < sfx.Length; i++)
            if(sfx[i].isPlaying)
                return true;
        
        return false;
    }

    public void StopSFX(int sfxToStop) => sfx[sfxToStop].Stop();
}
