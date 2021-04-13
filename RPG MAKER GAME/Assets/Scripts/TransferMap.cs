using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferMap : MonoBehaviour
{

    public string transferMapName; //이동할 맵의 이름

    public Transform target;
    public BoxCollider2D targetBound;

    private PlayerManager thePlayer;
    private CameraManager theCamera;
    private FadeManager theFade;

    // Start is called before the first frame update
    void Start()
    {
        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            StartCoroutine(TransferCorouine());
        }
    }

    IEnumerator TransferCorouine()
    {
        theFade.FadeOut();

        yield return new WaitForSeconds(1f);
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound);

        thePlayer.transform.position = target.transform.position; //맵 이동
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        theFade.FadeIn();
    }
}
