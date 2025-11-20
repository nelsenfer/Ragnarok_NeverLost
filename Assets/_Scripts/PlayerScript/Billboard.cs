using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        // Script ini memaksa object untuk meniru rotasi kamera
        // Jadi dia akan selalu tegak lurus menghadap layar
        transform.rotation = Camera.main.transform.rotation;
    }
}