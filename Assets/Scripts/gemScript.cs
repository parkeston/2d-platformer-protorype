using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gemScript : MonoBehaviour {

    private static int cratecounter = 0;


    void Start()
    {
        cratecounter++;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            cratecounter--;
            Debug.Log("Crate collected!");
            if (cratecounter == 0)
                Debug.Log("You win!");
            Destroy(gameObject);
        }
    }

}
