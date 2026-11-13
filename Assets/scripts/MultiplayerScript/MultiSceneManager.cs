using Mirror;

public class MultiSceneManager : NetworkManager
{
    public void OpenScene(string sceneName)
    {
        if (NetworkServer.active)
        {
            ServerChangeScene(sceneName);
        }
    }
}