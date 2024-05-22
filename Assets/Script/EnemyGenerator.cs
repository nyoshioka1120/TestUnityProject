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

    public GameObject Generate(string _name, Vector3 _pos)
    {
        switch(_name)
        {
            case "E_Mite":
            {
                GameObject obj = Instantiate(EnemyMite) as GameObject;
                obj.transform.position = _pos;
                obj.GetComponent<EnemyBase>().SetUID();
                return obj;
            }
            default:
            {
                break;
            }
        }

        return null;
    }
}
