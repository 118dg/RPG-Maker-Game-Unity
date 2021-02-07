using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//씬 이동시 bound 설정하는 스크립트
public class Bound : MonoBehaviour
{
    private BoxCollider2D bound;

    private CameraManager theCamera;

    // Start is called before the first frame update
    void Start()
    {
        bound = GetComponent<BoxCollider2D>();
        theCamera = FindObjectOfType<CameraManager>();
        theCamera.SetBound(bound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
