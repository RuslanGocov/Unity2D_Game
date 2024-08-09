using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
   private Vector3 offset = new Vector3(0f, 0f, -1f);
   private float smoothTime = 0.25f;
   private Vector3 velocity =  Vector3.zero;

   [SerializeField] private Transform target;


   void Update()
   {
      Vector3 targetPosiition = target.position + offset;
      transform.position = Vector3.SmoothDamp(transform.position, targetPosiition,ref  velocity, smoothTime);
   }
}
