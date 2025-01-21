using System;
using UnityEngine;

public class Exit : MonoBehaviour
{
    [SerializeField] private SceneController sceneController;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sceneController.LoadNextScene();
        }
    }
}
