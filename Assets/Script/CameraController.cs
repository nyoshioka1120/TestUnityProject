using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }

        Vector3 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x, transform.position.y, transform.position.z);

        if(transform.position.x < 0f)
        {
            transform.position = new Vector3(0.0f, transform.position.y, transform.position.z);
        }
    }

    public void SetPlayer()
    {
        if(player) return;

        player = GameObject.Find("Player(Clone)");
        Update();
    }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if(other.gameObject.tag == "Enemy")
    //     {
    //         other.gameObject.GetComponent<EnemyBase>().SetActive(true);
    //     }
    // }

    // void OnTriggerExit2D(Collider2D other)
    // {
    //     if(other.gameObject.tag == "Enemy")
    //     {
    //         other.gameObject.GetComponent<EnemyBase>().SetActive(false);
    //     }
    // }
}
