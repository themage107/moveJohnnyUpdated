using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveJohnnyUpdated : MonoBehaviour {

    // animation related variables
    int animState;
    public Animator jAnim;
    int rndInt;
    bool startRandom;

    // add or remove control of the Johnny Object
    public bool canMove;

    // physics pieces
    float maxSpeed = 12.75f;
    public float moveForce = 365f;

    // johnnypieces
    public Rigidbody2D johnny;
    float speed = 4.5f;
    SpriteRenderer j;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ceilingCheck;

    bool grounded;
    bool hanging;
    bool climbing;

    // input
    float h;
    float v;

    // commonly used vectors
    Vector3 forwardFacing = new Vector3(0, 90f, 0);
    Vector3 reverseFacing = new Vector3(0, 263f, 0);

    Vector3 wallForward = new Vector3(-2.17f, -0.713f, 16.61f);
    Vector3 wallReverse = new Vector3(2.17f, -0.713f, 16.61f);

    // stop collider force
    public bool obsTouching;
    public bool obsTouchingJohnny;
    public float obsX;

    // drop from ceiling
    WaitForSeconds hangingTime = new WaitForSeconds(0.25f);

    void Start()
    {
        // get the animation component
        animState = jAnim.GetInteger("animationStateJohnny");

        obsTouching = false;
        obsX = 0;

        grounded = false;
        hanging = false;
        climbing = false;

        canMove = false;

        // using this to allow the random idle states to show
        startRandom = false;
    }

    public void randomAnimation()
    {
        // this function is called from the event tab of Johnny's model
        runCheck();
    }

    public void resetAnimation()
    {
        // just make everything go idle
        jAnim.SetInteger("animationStateJohnny", 0);
    }

    public void runCheck()
    {
        // so here I'm running a random chance every completion of the default idle state to have a different animation
        // upon completion of this, they throw the resetAnimation function found in the events tab
        rndInt = Random.Range(0, 20);
        if (rndInt == 5)
        {
            jAnim.SetInteger("animationStateJohnny", 1);
        }
        if (rndInt == 10)
        {
            jAnim.SetInteger("animationStateJohnny", 2);
        }
        if (rndInt == 15)
        {
            jAnim.SetInteger("animationStateJohnny", 3);
        }
    }



    // movement controls

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canMove)
        {
            //use whichever is giving you input controller or keyboard

            // horizontal Movement
            if (Mathf.Abs(Input.GetAxis("HorizontalJS")) > Mathf.Abs(Input.GetAxis("Horizontal")))
            {
                if (Input.GetAxisRaw("HorizontalJS") > 0)
                {
                    h = 1;
                }
                else
                {
                    h = -1;
                }
            }
            else
            {
                h = Input.GetAxisRaw("Horizontal");
            }

            // if Johnny is running into the steel obs wall, just end the momentum in that direction possible
            if(grounded && obsTouching && h > 0 && johnny.transform.position.x < obsX || grounded && obsTouching && h < 0 && johnny.transform.position.x > obsX || ((obsTouchingJohnny && h < 0 && johnny.transform.position.x > obsX)) || ((obsTouchingJohnny && h > 0 && johnny.transform.position.x < obsX)))
            {
                h = 0;
            }

            // vertical movement
            if (Mathf.Abs(Input.GetAxis("VerticalJS")) > Mathf.Abs(Input.GetAxis("Vertical")))
            {
                if (Input.GetAxis("VerticalJS") > 0)
                {
                    v = -1;
                }
                else
                {
                    v = 1;
                }
            }
            else
            {
                v = Input.GetAxisRaw("Vertical");
            }

            // freeze the rigidbody from any rotations
            johnny.constraints = RigidbodyConstraints2D.FreezeRotation;


            // move Johnny
            if (!obsTouching || grounded || climbing || hanging || (obsTouching && h > 0 && johnny.transform.position.x > obsX) || ((obsTouching && h < 0 && johnny.transform.position.x < obsX)))
            {

                if (Mathf.Abs(h) > 0)
                {
                    jAnim.SetInteger("animationStateJohnny", 4);

                    // there was a state change, so we can reset the random idle
                    startRandom = true;
                }

                // if Johnny moves less than max
                if (h * johnny.velocity.x < maxSpeed)
                {
                    johnny.AddForce(Vector2.right * h * moveForce);
                }

                // if Johnny moves faster than max, clamp
                if (Mathf.Abs(johnny.velocity.x) > maxSpeed)
                {
                    johnny.velocity = new Vector2(Mathf.Sign(johnny.velocity.x) * maxSpeed, johnny.velocity.y);
                }

                if (h > 0)
                {
                    johnny.transform.localEulerAngles = forwardFacing;
                    wallCheck.localPosition = wallForward;
                }

                if (h < 0)
                {
                    johnny.transform.localEulerAngles = reverseFacing;
                    wallCheck.localPosition = wallReverse;
                }

                // no movement and grounded
                if (h > -0.15 && h < 0.15 && grounded)
                {
                    johnny.velocity = new Vector2(0, 0);

                    if (startRandom)
                    {
                        jAnim.SetInteger("animationStateJohnny", 0);

                        // stop it from forcing idle state 0
                        startRandom = false;
                    }
                }

                if (climbing)
                {
                    wallCheck.GetComponent<BoxCollider2D>().isTrigger = false;
                    // Johnny is doing the climbing animation
                    jAnim.SetInteger("animationStateJohnny", 5);                    
                    
                    if (v * johnny.velocity.y < maxSpeed)
                    {
                        johnny.AddForce(Vector2.up * v * moveForce);
                    }

                    //climbing max speed is 7.5
                    if (Mathf.Abs(johnny.velocity.y) > 7.5f)
                    {
                        johnny.velocity = new Vector2(johnny.velocity.x, Mathf.Sign(johnny.velocity.y * 7.5f));
                    }

                    //set the position of the climbing animation properly up/down
                    float localY;

                    if (v > 0.15)
                    {
                        
                    }
                    else
                    {
                        
                    }
                   

                    if (v > -0.15 && v < 0.15)
                    {
                        johnny.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;                        
                    }
                }

                if (hanging)
                {
                    jAnim.SetInteger("animationStateJohnny", 6);
                    johnny.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
                  
                    if (h < -0.15)
                    {
                       
                    }

                    if (h > 0.15)
                    {
                        
                    }

                    if (h > -0.15 && h < 0.15)
                    {                        
                        johnny.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX;
                    }

                }

                // this would be falling in air
                if (!grounded && !climbing && !hanging)
                {

                    //we'll update this for a falling animation, but for now it's the idle one
                    jAnim.SetInteger("animationStateJohnny", 7);

                    //stop the burst force that is happening
                    if (johnny.velocity.y > 9f)
                    {
                        Debug.Log("Johnny velocity y: " + johnny.velocity.y);
                        johnny.velocity = new Vector2(johnny.velocity.x, Mathf.Sign(johnny.velocity.y * 9f));
                    }
                }

                if (!climbing)
                {
                    wallCheck.GetComponent<BoxCollider2D>().isTrigger = true;
                }                

            }

        }
        else
        {
            //johnny can't move, so freeze everything
            johnny.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezePositionX;
        }
    }

    /*
    IEnumerator stopHanging()
    {
        johnny.constraints = RigidbodyConstraints2D.FreezeRotation;
        ceilingCheck.transform.localPosition = new Vector3(0, 0, 0);
        ceilingCheck.transform.localPosition = new Vector3(0, 1.7f, 0);
        yield return null;
    }
    */

    IEnumerator stopHanging()
    {
        johnny.constraints = RigidbodyConstraints2D.FreezeRotation;
        ceilingCheck.localPosition = new Vector3(0, 0, 0);
        yield return hangingTime;
        ceilingCheck.localPosition = new Vector3(0, 16.5f, 0);
    }

    void Update()
    {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        climbing = Physics2D.Linecast(transform.position, wallCheck.position, 1 << LayerMask.NameToLayer("Wall"));
        hanging = Physics2D.Linecast(transform.position, ceilingCheck.position, 1 << LayerMask.NameToLayer("Ceiling"));

        if (Input.GetKeyDown(KeyCode.Space) && hanging || Input.GetAxis("SubmitJS") > 0)
        {
            StartCoroutine("stopHanging");
        }

    }


}
