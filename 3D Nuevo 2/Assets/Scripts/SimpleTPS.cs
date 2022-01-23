using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTPS : MonoBehaviour
{
    public CharacterController character;
    public Transform cam;
    public Animator animator;
    public AudioSource swordSlash;
    public float speed;
    float initSpeed;
    float doubleSpeed;
    public float turnSmoothTime;
    private float turnSmoothVelocity;
    private float gravity = -9.81f;
    public GameObject helm;
    public GameObject ears;
    public GameObject hair;
    public float jumpHeight;
    bool canMove;
    bool canHit;
    //stuff
    Vector3 velocity;
    public Vector3 direction;
    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        initSpeed = speed;
        doubleSpeed = speed*2;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical  = Input.GetAxisRaw("Vertical");
        direction  = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.01f){
            
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);    

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            animator.SetBool("isMoving", true);
           if(canMove) character.Move(moveDir.normalized * speed * Time.deltaTime);
        } else animator.SetBool("isMoving", false);

        
     

         if(Input.GetButton("Fire1") && canHit){
            swordSlash.Play();
            canMove = false;
            canHit = false;
            animator.Play("Base Layer.Hit", 0, 0.25f);

        } 
         if(Input.GetKey(KeyCode.LeftShift)){

            speed = doubleSpeed;
            animator.SetBool("isRunning", true);

        } else{ 
        
            speed = initSpeed;
            animator.SetBool("isRunning", false);
                
        }

        if(Input.GetKey(KeyCode.E)){

            helm.SetActive(false);
            ears.SetActive(true);
            hair.SetActive(true);

        } 
        if(Input.GetKey(KeyCode.Q)){

            helm.SetActive(true);
            ears.SetActive(false);
            hair.SetActive(false);

        } 

        if(!AnimatorIsPlaying("Base Layer.Hit") && !AnimatorIsPlaying("Base Layer.Jump")){
            canHit = true;
        }
        if(!AnimatorIsPlaying("Base Layer.Hit")){
            canMove = true;
        }

    
        if(character.isGrounded && Input.GetButton("Jump")){
            velocity.y += jumpHeight;     
            canHit = false;
            canMove = false;
            animator.Play("Base Layer.Jump", 0, 0.25f);
        }   

        velocity.y += gravity * Time.deltaTime;
        character.Move(velocity * Time.deltaTime);


    }

    bool AnimatorIsPlaying(string stateName){
            return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
     }

 
 
    }

