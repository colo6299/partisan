using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPartisan : MonoBehaviour
{
    public Vector3 inputDirection;
    public float acceleration = 10;
    public float deceleration = 0.1f;
    public float decelerationFraction = 0.3f;
    public float maxNormalSpeed = 10;
    public float maxBoostSpeed = 20;
    public float boostAcc = 30;
    public Vector3 velocity;

    public Animator animator;

    public bool boosting;
    public FireController fireControl;
    public GameObject cameraHolder;
    public Harvester harvester;
    public float hvUnlockTime = 0;
    private float hvStartLock = 1f;
    private float hvEndLock = 0.5f;
    private float hvTime = 0;
    private bool hvExit = false;
    private bool hvSpinning = false;
    private bool harvestInput = false;

    private bool harvesting;
    private float harvestDivider;
    private float speedMult = 1;

    private bool connected = false;
    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (!connected) { return; }

        CheckControlOverrides();
        CheckHarvesting();
        CheckBoosting();
        CheckFiring();
        LookRotate();
        Movement();
    }

    public void SetConnect(bool cnct_state)
    {
        connected = cnct_state;
        cameraHolder.SetActive(connected);
    }

    private void CheckControlOverrides()
    {
        if (Input.GetButtonDown("Harvest"))
        {
            harvestInput = !harvestInput;
        }
    }

    private void CheckHarvesting()
    {
        if ((Time.time > hvEndLock) & hvExit)
        {
            hvExit = false;
        }
        if (Input.GetButton("Harvest"))
        {
            if (!hvExit)
            {
                if (!harvesting)
                {
                    hvTime = Time.time;
                    hvUnlockTime = Time.time + hvStartLock + hvEndLock;
                    speedMult = 1 / 6;
                    harvesting = true;
                    animator.SetBool("flip", true);
                }
                if (Time.time - hvTime >= hvStartLock)
                {
                    harvester.SetState(true);
                    hvSpinning = true;
                    hvUnlockTime = Time.time + hvEndLock;
                }
            }
        }
        else
        {
            if (Time.time - hvTime >= hvStartLock)
            {
                harvester.SetState(false);
                hvExit = true;
            }
        }
        if (Input.GetButtonUp("Harvest") & harvesting)
        {
            speedMult = 1;
            animator.SetBool("flip", false);
            harvesting = false;
        }
    }

    private void CheckBoosting()
    {

        if (Input.GetButton("Boost") & !harvesting)
        {
            boosting = true;
        }
        else
        {
            boosting = false;
        }
    }

    private void CheckFiring()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!harvesting & (Time.time > hvUnlockTime)) { fireControl.TryFire(); }
            else { }
        }
    }

    private void LookRotate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float leftRight = Input.GetAxis("Mouse X") * 200;
        transform.Rotate(transform.up, leftRight * Time.deltaTime);
    }

    private void Movement()
    {
        CalculateInputDirection();
        SlowDownMult();
        SlowDownLinear();
        SlewAndClamp();

        transform.position += velocity * Time.deltaTime;
    }

    private void CalculateInputDirection()
    {
        //calculate the aggregate direction of player inputs
        Vector3 tempMove = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        tempMove.Normalize();
        tempMove = transform.TransformDirection(tempMove);
        inputDirection = tempMove;
    }

    private void SlowDownMult()
    {
        Vector3 aggregate = velocity * decelerationFraction * Mathf.Sqrt(speedMult);
        velocity = Vector3.Lerp(velocity, aggregate, Time.deltaTime);
    }

    private void SlowDownLinear() //feels awful to use _ALONE_
    {
        float currentSpeed = velocity.magnitude;
        currentSpeed -= deceleration * Time.deltaTime;
        if (currentSpeed < 0) { currentSpeed = 0; }
        velocity.Normalize();
        velocity *= currentSpeed;
    }

    private void SlewAndClamp()
    {
        float currentSpeed = velocity.magnitude;
        if (boosting)
        {
            Vector3 boostVector = transform.forward * boostAcc * Time.deltaTime;
            velocity += boostVector;
        }
        else
        {
            if (!harvesting) { velocity += InputToVector3() * Time.deltaTime * acceleration; }
            else { velocity += InputToVector3() * Time.deltaTime * acceleration / 3; }
        }

        //if somehow the player ends up going crazy fast
        if (currentSpeed > maxBoostSpeed)
        {
            Vector3.ClampMagnitude(velocity, maxBoostSpeed);
        }

        if (boosting) { BoostClamp(currentSpeed); }
        else { NormalClamp(currentSpeed); }
    }

    private Vector3 InputToVector3()
    {
        return new Vector3(inputDirection.x, 0, inputDirection.z);
    }

    private void NormalClamp(float currentSpeed)
    {
        if (currentSpeed > maxNormalSpeed*speedMult)
        {
            Vector3.ClampMagnitude(velocity, currentSpeed);
        }
        else if (currentSpeed <= maxNormalSpeed)
        {
            Vector3.ClampMagnitude(velocity, maxNormalSpeed*speedMult);
        }
    }

    private void BoostClamp(float currentSpeed)
    {
        Vector3.ClampMagnitude(velocity, maxBoostSpeed);
    }
}
