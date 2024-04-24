using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] GameObject EnemyMite;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.Alpha1))
        {
            var obj = Instantiate(EnemyMite);
            obj.transform.position = new Vector3(5.0f, -2.0f, 0);
        }
    }
}
