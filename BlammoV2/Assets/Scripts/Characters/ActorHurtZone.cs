using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorHurtZone : MonoBehaviour
{

    Actor ParentActor;

    private void Awake()
    {
        ParentActor = GetComponent<Actor>();
        if (ParentActor == null)
        {
            ParentActor = GetComponentInParent<Actor>();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Dart d = collision.gameObject.GetComponent<Dart>();
        if(d!=null && d.ActiveDamage)
        {
            ParentActor.Hit();
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
