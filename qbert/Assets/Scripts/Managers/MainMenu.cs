using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  private string selectMode;
  private GameObject Instructions2D;
  private GameObject Instructions3D;


  void Start()
  {
    Cursor.lockState = CursorLockMode.None;
    Instructions2D = GameObject.Find("2DInstructions");
    Instructions3D = GameObject.Find("3DInstructions");
    Instructions2D.SetActive(false);
    Instructions3D.SetActive(false);
  }

  public void QuitGame()
  {
    Application.Quit();
  }

  public void setSelectMode(string sm)
  {
    selectMode = sm;
  }

  public void loadSelectMode(string gamemode)
  {
    switch (selectMode)
    {
      case "controls":
        if (gamemode == "2D")
          Instructions2D.SetActive(true);
        else
          Instructions3D.SetActive(true);
        break;
      case "play":
        if (gamemode == "2D")
          SceneManager.LoadScene(1);
        else
          SceneManager.LoadScene(2);
        break;
    }
  }
}
