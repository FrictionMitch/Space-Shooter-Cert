using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    Button mainMenuButton;
    // Start is called before the first frame update
    void Start()
    {
        mainMenuButton.onClick.AddListener(LoadMain);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadMain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
