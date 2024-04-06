using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class ActorAnimator : MonoBehaviour
{


    Actor Owner;

    public Animator Anim;

    public string Idle = "Idle";
    public string Running = "Run";
    public string Falling = "Fall";
    public string StandUpFaceDown = "StandUpFaceDown";
    public string StandUpFaceUp = "StandUpFaceUp";

    List<Collider> RBodyColliders = new List<Collider>();
    Rigidbody[] RBodies;


    public Transform RagdollRoot;
    Vector3 RagdollRootBaseOffset;
    Vector3 RagdollBaseRot;
    Actor RagdollRootParent;

    public float StandUpTransitionTime = .25f;


    public Transform AnimDefParent, DrawParent;
    public List<AnimationSkeletonMatch> SkelBones = new List<AnimationSkeletonMatch>();


    private void Awake()
    {
        Owner = GetComponentInParent<Actor>();
        RBodies = GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < RBodies.Length; i++)
        {
            Collider col = RBodies[i].GetComponent<Collider>();
            if(col!=null)
            {
                RBodyColliders.Add(col);
            }
        }
    }

    public void RegisterRootOffset(Actor movingParent)
    { 
        RagdollRootBaseOffset = RagdollRoot.position - movingParent.thisTransform.position;
        RagdollRootParent = movingParent;
        RagdollBaseRot = RagdollRoot.localRotation.eulerAngles;
    }



    bool StandUpTransitioning = false;
    bool RagDolledSet = false;
    void Ragdoll(bool on, bool force)
    {
        if (RagDolledSet!=on || force)
        {
            RagDolledSet = on;
            //Anim.enabled = !on;

            /*for(int i=0; i<RBodyColliders.Count; i++)
            {
                RBodyColliders[i].enabled = on;
            }*/
            for(int i=0; i< RBodies.Length; i++)
            {
                RBodies[i].isKinematic = !on;
            }

            Owner.thisRigidbody3D.isKinematic = on;



            if(!RagDolledSet) //save last pos
            {
                for (int i = 0; i < SkelBones.Count; i++)
                {
                    if (SkelBones[i].DrawBone != null)
                    {
                        SkelBones[i].LastRagLocalP= SkelBones[i].DrawBone.localPosition;
                        SkelBones[i].LastRagRot = SkelBones[i].DrawBone.localRotation;

                    }

                }
                StandUpTransitioning = true;
            }

        }
    }


    public bool IsFaceUp()
    {
        return Vector3.Dot(RagdollRoot.up, Vector3.up) < 0;
    }


    CharacterState LastSetState = CharacterState.None;

    public void SetState(CharacterState stat, bool force = false)
    {
        if (stat == CharacterState.DeadRagdoll)
        {
            Ragdoll(true, force); 
            if (LastSetTrigger != null)
            {
                Anim.ResetTrigger(LastSetTrigger);
                LastSetTrigger = null;
            }
            LastSetState = stat;
        }
        else
        {
            if (force || stat != LastSetState) //so much cleanrer if we never re-re set here
            {
                Ragdoll(false, force);
                ChangeAnimation(stat);
            }
        }
    }

    public void CompleteEvent()
    {
        Owner.CompleteState(LastSetState);
    }

    void ChangeAnimation(CharacterState stat)
    {
        string trigger = LastSetTrigger;
        switch (stat)
        {
            case CharacterState.Falling:
                {
                    trigger = Falling;
                    break;
                }
            case CharacterState.Running:
                {
                    trigger = Running;
                    break;
                }
            case CharacterState.StandUp:
                {
                    if (IsFaceUp())
                    {
                        trigger = StandUpFaceUp;
                    }
                    else
                    {
                        trigger = StandUpFaceDown;
                    }
                    break;
                }
            case CharacterState.Idle:
                {
                    trigger = Idle;
                    break;
                }
        }
        if(trigger!= LastSetTrigger)
        {
            SetTrigger(stat, trigger);
        }
    }



    string LastSetTrigger = null;
    float LastSetTriggerTime = 0;
    void SetTrigger(CharacterState stat, string trig)
    {
        if(LastSetTrigger!=null)
        {
            Anim.ResetTrigger(LastSetTrigger);
        }
        LastSetState = stat;
        LastSetTrigger = trig;
        LastSetTriggerTime = Time.time;
        Anim.SetTrigger(trig);
    }


    private void LateUpdate()
    {
        ///So on LateUpdate() after the animation position is calculated:
        //To get the animated bone position, we have to look at the transform on LateUpdate,
        //but for a real humanoid rig, it should be accessible using Animator.GetBoneTransform
        //(https://docs.unity3d.com/ScriptReference/Animator.GetBoneTransform.html)


        //lets copy the anim over?
        if (LastSetState != CharacterState.DeadRagdoll)
        {
            if (LastSetState == CharacterState.StandUp)
            {
                if (StandUpTransitioning)
                {
                    float stateDegree = (Time.time - LastSetTriggerTime) / StandUpTransitionTime;
                    if(stateDegree >= 1)
                    {
                        stateDegree = 1;
                        StandUpTransitioning = false;
                    }
                    for (int i = 0; i < SkelBones.Count; i++)
                    {
                        if (SkelBones[i].DrawBone != null)
                        {
                            SkelBones[i].DrawBone.localPosition = Vector3.Lerp(SkelBones[i].LastRagLocalP, SkelBones[i].AnimDefBone.localPosition, stateDegree);
                            SkelBones[i].DrawBone.localRotation = Quaternion.Slerp(SkelBones[i].LastRagRot, SkelBones[i].AnimDefBone.localRotation, stateDegree);
                        }

                    }
                }
            }
            else
            {
                for (int i = 0; i < SkelBones.Count; i++)
                {
                    if (SkelBones[i].DrawBone != null)
                    {
                        SkelBones[i].DrawBone.localPosition = SkelBones[i].AnimDefBone.localPosition;
                        SkelBones[i].DrawBone.localRotation = SkelBones[i].AnimDefBone.localRotation;
                    }

                }
            }
        }    
        else
        {
            UpdateRagdollParentPos();
        }


        //Debug.Log("eh");
        /*

        //Interpolation of position and rotation for each bone, using a blendSpeed variable to be able to tweak the strenght of the blend:
        Anim.bone
        foreach (Bones b in skeleton)
        {
            b.transform.position = Vector3.Lerp(b.transform.position, b.ragdolledBonePosition, blendSpeed);
            b.transform.rotation = Quaternion.Slerp(b.transform.rotation, b.ragdolledBonePosition, blendSpeed);
        }*/

    }

    public void UpdateRagdollParentPos()
    {

        //warp pos for parent
        Vector3 orig = RagdollRootParent.thisTransform.position;
        Vector3 baseP = RagdollRoot.position;
        Vector3 newBase = RagdollRoot.position + RagdollRootBaseOffset;
        newBase.y = RagdollRootParent.SafeSetGround(newBase.y);

        RagdollRootParent.thisTransform.position = newBase;
        RagdollRoot.position = baseP; //reset!  make sure we dont wander!

        
        //update rot

    }


    public void StepEvent()
    {
    }

    public void BuildAnimDrawMapping()
    {
        SkelBones.Clear();
        //build everything for anim parent
        Transform[] animDefBones = AnimDefParent.GetComponentsInChildren<Transform>();
        for(int i=0; i<animDefBones.Length; i++)
        {
            AnimationSkeletonMatch newB = new AnimationSkeletonMatch();
            newB.AnimDefBone = animDefBones[i];
            SkelBones.Add(newB);
        }
        //match everything for draw that we can
        Transform[] drawBones = DrawParent.GetComponentsInChildren<Transform>();
        for (int i = 0; i < drawBones.Length; i++)
        {
            for(int j=0; j<SkelBones.Count; j++)
            {
                if(SkelBones[j].AnimDefBone.name == drawBones[i].name)
                { //we got some magic
                    SkelBones[j].DrawBone = drawBones[i];
                    break;
                }
            }
        }




    }


}


public enum CharacterState
{
    None,
    Idle,
    DeadRagdoll,
    Running,
    Falling,
    StandUp
}

[System.Serializable]
public class AnimationSkeletonMatch
{
    public Transform AnimDefBone,DrawBone;
    public Quaternion LastRagRot;
    public Vector3 LastRagLocalP;
}







#if UNITY_EDITOR

[CanEditMultipleObjects]
[CustomEditor(typeof(ActorAnimator))]
public class BushEditor : Editor
{

    ActorAnimator w;

    void OnEnable()
    {
        w = (ActorAnimator)target;
    }
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Build Anim/Draw Mapping"))
        {
            w.BuildAnimDrawMapping();
            EditorUtility.SetDirty(w);
            EditorUtility.SetDirty(w.gameObject);
        }
        base.OnInspectorGUI();
    }
}

#endif