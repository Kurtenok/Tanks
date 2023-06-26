using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Vector2 offset;
   private void Update() {
    transform.localPosition=(Input.mousePosition/2)*2- new Vector3(offset.x,offset.y,0);
    
   }
}
