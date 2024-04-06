using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public ActorManager ActMan;

    CapsuleCollider thisCapsuleCollider3D;


    public ActorAnimator Anim;

    [System.NonSerialized]
    public Rigidbody thisRigidbody3D;
    [System.NonSerialized]
    public Transform thisTransform;

    public float Accel = 5;
    public float MaxVel = 5;
    float CurVelocity = 0;

    float RespawnTimer = -1;

    public float RespawnTime = 2;

    public float RotSpeed = 10;



    private void Awake()
    {
        thisTransform = transform;
        thisRigidbody3D = GetComponent<Rigidbody>();
        thisCapsuleCollider3D = GetComponent<CapsuleCollider>();
    }


    GlobalConstants GConsts;
    PhysicsController PhysicsCont;
    // Start is called before the first frame update
    void Start()
    {
        GConsts = GlobalConstants.Instance;
        PhysicsCont = PhysicsController.Instance;
        Anim.RegisterRootOffset(this);
        Respawn();
    }

    CharacterState CurState = CharacterState.None;
    float CurStateLastSetTime = 0;
    void SetState(CharacterState nState, bool force = false)
    {
        if(CurState == CharacterState.DeadRagdoll && nState != CharacterState.DeadRagdoll)
        {
           Anim.UpdateRagdollParentPos();            
        }

        CurStateLastSetTime = Time.time;
        CurState = nState;
        Anim.SetState(nState, force);
    }


    void ClearAngularVelocity()
    {

        Vector3 vel = thisRigidbody3D.angularVelocity;
        vel.x = vel.y = vel.z = 0;
        thisRigidbody3D.angularVelocity = vel;
    }
    void ClearVelocity()
    {

        Vector3 vel = thisRigidbody3D.velocity;
        vel.x = vel.z = 0;
        thisRigidbody3D.velocity = vel;
        CurVelocity = 0;
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Pos " + thisTransform.position + " " + thisRigidbody3D.velocity * 100);

        
        if (CurState == CharacterState.DeadRagdoll)
        {
            Anim.UpdateRagdollParentPos();
        }

        if (RespawnTimer >= 0)
        {
            RespawnTimer -= Time.deltaTime;
            if (RespawnTimer < 0)
            {
                //Respawn();
                SetState(CharacterState.StandUp, true);
            }
            return;
        }
        ClearAngularVelocity();
        if (CurState == CharacterState.StandUp)
        {
            ClearVelocity();
            return;
        }


        //udate rot
        Vector3 diff = thisTransform.position - ActMan.Player.position;
        diff.y = 0;
        float ang = Vector3.SignedAngle(Vector3.back, diff, Vector3.up);
        thisTransform.localRotation = Quaternion.RotateTowards(thisTransform.localRotation, Quaternion.Euler(0, ang, 0), RotSpeed * Time.deltaTime);

        //update state
        if (!IsGrounded)
        {
            SetState(CharacterState.Falling);
        }
        else
        {
            SetState(CharacterState.Running);
        }




        if (CurState == CharacterState.Idle)
        {
            CurVelocity = 0;
        }
        else if(CurState == CharacterState.Running || CurState == CharacterState.Falling)
        {
            CurVelocity = Mathf.Min(CurVelocity + Time.deltaTime * Accel, MaxVel);
        }
        float useVel = CurVelocity;
        if(CurState == CharacterState.Falling)
        {
            useVel *= PhysicsCont.FallSpeedFactor;
        }
        Vector3 run = thisTransform.forward * useVel;
        Vector3 vel = thisRigidbody3D.velocity;
        vel.x = run.x;
        vel.z = run.z;
        thisRigidbody3D.velocity = vel;


    }

    private void FixedUpdate()
    {
        UpdateJump(false);
    }

    public void Respawn()
    {
        thisTransform.position = ActMan.GetRespawnPosition();
        SetState(CharacterState.Idle,true);
    }


    public void Hit()
    {
        SetState(CharacterState.DeadRagdoll);
        RespawnTimer = RespawnTime;
    }


    public void CompleteState(CharacterState stat)
    {
        if(stat != CurState)
        {
            return;
        }
        switch(stat)
        {
            case CharacterState.StandUp:
                {

                    SetState(CharacterState.Idle);
                    break;
                }
        }
    }



    [Header("Jump Settings")]
    public bool IsGrounded = false;
    public bool ForwardObstacle = false;
    float MinFallVel = 0;
    enum JumpRisingState
    {
        None,
        Req
    }
    public float LastOnGroundY = 0;
    JumpRisingState JumpRisingS = JumpRisingState.None;
    void UpdateJump(bool justjumped)
    {



        bool wasGrounded = IsGrounded;
        IsGrounded = false;
        ForwardObstacle = false;
        //moved from update
        //IceFactor = 0;
        float prad = thisCapsuleCollider3D.bounds.size.x / 2.1f;  //was 4, but the deer woman was stuck on a ledge and thought she was forever falling.  we need a more "Accurate" size then
        if (JumpRisingS == JumpRisingState.Req && (justjumped || thisRigidbody3D.velocity.y > .20f)) //dont bother ground check if jumping
        {
            //Debug.LogWarning("Jumping " + thisRigidbody3D.velocity.y);
        }
        else
        {
            JumpRisingS = JumpRisingState.None;

            RaycastHit hit;
            //if (Physics.CheckCapsule(thisCollider3D.bounds.center,
            //                new Vector3(thisCollider3D.bounds.center.x, thisCollider3D.bounds.min.y + prad - GroundDistance, thisCollider3D.bounds.center.z),
            //                prad, GeneralWorldMask))


            //alert, alert!
            //if we are already embedded in the thing, we are doomzor, it wont find the collision
            //so the radius has to be a BIT smaller, hence the .95f

            if (DoesActorHit(out hit, Vector3.down, PhysicsCont.GroundDistance))
            {


                IsGrounded = true;
                LastOnGroundY = thisCapsuleCollider3D.bounds.min.y;            
            }
        }


        if (thisRigidbody3D.velocity.y < 0)
        {
            if (IsGrounded || wasGrounded)
            {
                thisRigidbody3D.velocity += Vector3.up * Physics.gravity.y * (PhysicsCont.GroundedFallAddl) * Time.fixedDeltaTime;
            }
            else
            {
                thisRigidbody3D.velocity += Vector3.up * Physics.gravity.y * (PhysicsCont.FallAddl) * Time.fixedDeltaTime;
            }
        }
        else if (thisRigidbody3D.velocity.y > 0)
        {
            if (false)//(!ActorBrain.JumpHeld())
            {
                thisRigidbody3D.velocity += Vector3.up * Physics.gravity.y * (PhysicsCont.LowJumpAddl) * Time.fixedDeltaTime;
            }
        }


        if (!IsGrounded)
        {
            MinFallVel = Mathf.Min(MinFallVel, thisRigidbody3D.velocity.y);
        }
        if (IsGrounded && !wasGrounded)
        {
            CurVelocity = 0;
            //landed!
            if (MinFallVel < -.5f)
            {
                Anim.StepEvent();

            }
            MinFallVel = 0;


        }

    }

    public float SafeSetGround(float desY)
    {        
        if(IsGrounded && desY < LastOnGroundY + thisCapsuleCollider3D.height/2f)
        {
            return LastOnGroundY + thisCapsuleCollider3D.height / 2f;
        }
        return desY;

    }

    public bool DoesActorHit(out RaycastHit hit, Vector3 dir, float distance, float radFactor = .98f)
    {
        Vector3 capsuleTop = thisCapsuleCollider3D.bounds.center;
        Vector3 capsuleBottom = thisCapsuleCollider3D.bounds.center;
        float scal = 1;
        float baseRad = thisCapsuleCollider3D.radius * scal;
        capsuleTop.y = .01f + thisCapsuleCollider3D.bounds.max.y - baseRad;
        capsuleBottom.y = .01f + thisCapsuleCollider3D.bounds.min.y + baseRad;
        float rad = baseRad * radFactor;
        if (scal > 1) //big spider wasnt seeing he was on the ground, scale could mess up the "buffer" zone
        {
            distance *= scal;
        }
        return Physics.CapsuleCast(capsuleTop,
                              capsuleBottom,
                       rad, dir, out hit, distance, GConsts.GeneralWorldMask, QueryTriggerInteraction.Ignore);
    }

}
