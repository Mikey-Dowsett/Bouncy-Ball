using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager Instance
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
