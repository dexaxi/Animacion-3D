using UnityEngine;
using System.Collections;

public class WalkScript : MonoBehaviour
{

    // Use this for initialization
    CharacterController cc;
    AudioSource aso;
    void Start()
    {
        if(aso == null) aso = GetComponent<AudioSource>();
        if(cc == null) cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cc.isGrounded == true && cc.GetComponent<SimpleTPS>().direction.magnitude > 0.1f)
        {             aso.volume = Random.Range(0.1f, 0.2f);
           if(!cc.GetComponent<SimpleTPS>().animator.GetBool("isRunning")) aso.pitch= Random.Range(0.8f, 1.1f); else  aso.pitch= Random.Range(1.2f,1.7f); 
            if(!aso.isPlaying && cc.velocity != Vector3.zero) aso.Play();
        } 
    }
}