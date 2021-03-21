using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    static public BGMManager instance;

    public AudioClip[] clips; // 배경음악들
    
    private AudioSource source;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f); //new 여러번 호출 줄이기 - 성능 향상

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play(int _playMusicTrack)
    {
        source.clip = clips[_playMusicTrack];
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines(); //FadeOut과 FadeIn 중복 방지. 모든 Corouine 멈추고 FadeOutCoroutine만 실행.
        StartCoroutine(FadeOutMusicCoroutine());
    }

    IEnumerator FadeOutMusicCoroutine() //MovingObject 스크립트 참고. 이거 공부 필요한 개념.
    {
        for (float i = 1.0f; i >= 0f; i -= 0.01f) //볼륨 점점 줄어듬. for문 100번 반복.
        {
            source.volume = i;
            yield return waitTime;
        }
    }

    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInMusicCoroutine());
    }

    IEnumerator FadeInMusicCoroutine() //MovingObject 스크립트 참고. 이거 공부 필요한 개념.
    {
        for (float i = 0f; i <= 1.0f; i += 0.01f) //볼륨 점점 커짐. for문 100번 반복.
        {
            source.volume = i;
            yield return waitTime;
        }
    }
}
