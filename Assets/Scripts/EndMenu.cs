using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    public void GoToStartMenu()
    {
        GameManager.Instance.GoToMenu();
    }

    public void Exit()
    {
        GameManager.Instance.Exit();
    }
}
