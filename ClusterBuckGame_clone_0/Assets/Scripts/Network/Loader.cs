using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public static class Loader
{

    public enum Scene {
        MainMenu,
        PlayerCustomisation,
        GameScene
    }

    private static Scene targetScene;


    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.MainMenu.ToString());
    }
    public static void LoadNetwork(Scene targetScene)
    {
        //Loader.targetScene = targetScene;
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
}
