using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PasarNivel : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D collision){
    if(collision.CompareTag("Player")){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }
   }
}
