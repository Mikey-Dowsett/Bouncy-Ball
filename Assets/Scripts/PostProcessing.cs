using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessing : MonoBehaviour
{
    private static PostProcessing instance;
    public static PostProcessing Instance
    {
        get { return instance; }
    }

    private void Start(){
        DontDestroyOnLoad(gameObject);

        if(instance == null){
            instance = this;
        } else {
            GameObject.Destroy(gameObject);
        }
    }
}
