using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{



    //Se define una amplitud y una frecuencia del movimiento 
    public float amplitude = 0.5f;
    public float frequency = 1f;
 
    // Variables de almacenamiento de posición
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();

    //Objeto del jugador al que tiene que mirar
    public GameObject playerController;

     void Start () {

         //encuentra al jugador, almacena la posición propia
         if (playerController == null)  playerController = FindObjectOfType<SimpleFPS>().gameObject;
        posOffset = transform.position;
    }
     
    void Update () {

        
        // flota utilizando el seno
        tempPos = posOffset;
        tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
        //calculamos la dirección del jugador y hacemos un lerp para mirar en su dirección
        Vector3 dir = playerController.transform.position - transform.position;
         dir.y = 0; 
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 1 * Time.deltaTime);
        transform.position = tempPos;
    }
}
    
