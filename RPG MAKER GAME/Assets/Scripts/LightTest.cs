using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTest : MonoBehaviour
{
    public GameObject go;

    private bool flag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag)
        {
            go.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
