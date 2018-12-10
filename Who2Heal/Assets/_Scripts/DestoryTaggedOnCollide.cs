using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryTaggedOnCollide : MonoBehaviour
{
    [SerializeField]
    private string TargetTag;

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals(TargetTag))
        {
            Destroy(collision.gameObject);
        }
    }

}
