using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField]
    private AudioSource bowlerthrowaudio;
    [SerializeField]
    private AudioSource ballhitgroundaudio;
    [SerializeField]
    private AudioSource bathitaudio;
    [SerializeField]
    private AudioSource stumpshitaudio;

    // Start is called before the first frame update
    void Start()
    {
        BowlPlayer.OnThrownBall += PlayBowlerSound;
        BatsmanPlayer.onBallHit += PlayBatHitSound;
        Ball.onTouchGround += PlayBallHitGround;
        Ball.onStumpsHit += PlayStumpsHitSound;
    }

    private void OnDestroy()
    {
        BowlPlayer.OnThrownBall -= PlayBowlerSound;
        BatsmanPlayer.onBallHit -= PlayBatHitSound;
        Ball.onTouchGround -= PlayBallHitGround;
        Ball.onStumpsHit -= PlayStumpsHitSound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBowlerSound(float secs)
    {
        bowlerthrowaudio.Play();
    }

    public void PlayBallHitGround(Vector3 pos)
    {
        ballhitgroundaudio.pitch = Random.Range(0.9f, 1.1f);
        ballhitgroundaudio.Play();
    }

    public void PlayBatHitSound(Transform pos)
    {
        bathitaudio.pitch = Random.Range(0.75f, 0.9f);
        bathitaudio.Play();
    }

    public void PlayStumpsHitSound()
    {
        stumpshitaudio.Play();
    }
}
