using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{

    public string transferMapName; //이동할 맵의 이름

    public Transform target;

    private MovingObject thePlayer;
    private CameraManager theCamera;

    //public bool flag; //씬 이동, 맵 이동 선택하게 할 때 사용

    // Start is called before the first frame update
    void Start()
    {
        //if (!flag)
            theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<MovingObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            thePlayer.currentMapName = transferMapName;

            //if (flag)
                //SceneManager.LoadScene(transferMapName); //씬 이동
            //else {
                thePlayer.transform.position = target.transform.position; //맵 이동
                theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
            //}
            
        }
    }
}
