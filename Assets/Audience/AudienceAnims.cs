using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceAnims : MonoBehaviour
{
    public enum CrowdState { idle, applause, applause2, celebration, celebration2 }
    public CrowdState crowdState;
    private Animator anim;
     
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        BatsmanPlayer.onBallHit += Celebrate;
        Ball.onBallMissed += Applause;
        Ball.onTouchGround += Applause2;
        Ball.onStumpsHit += Celebrate2;

        if(crowdState==CrowdState.idle)
        {
            anim.Play("idle");
        }
    }

    private void OnDestroy()
    {
        BatsmanPlayer.onBallHit -= Celebrate;
        Ball.onBallMissed -= Applause;
        Ball.onTouchGround -= Applause2;
        Ball.onStumpsHit -= Celebrate2;
    }


    public void Applause()
    {
        crowdState = CrowdState.applause;
        anim.Play("applause");
    }

    public void Celebrate(Transform pos)
    {
        crowdState = CrowdState.celebration;
        anim.Play("celebrate");
    }

    public void Applause2(Vector3 pos)
    {
        crowdState = CrowdState.applause2;
        anim.Play("applause2");
    }

    public void Celebrate2()
    {
        crowdState = CrowdState.celebration2;
        anim.Play("celebrate2");
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
