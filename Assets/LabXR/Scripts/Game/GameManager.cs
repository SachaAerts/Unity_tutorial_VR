public class GameManager : MonoSingleton<GameManager>
{
    /// <summary>
    /// Start of the whole Game
    /// Be careful when considering adding a Start method in another behaviour
    /// </summary>
    private void Start()
    {
        PlayerController.Singleton.Init();
    }

    /// <summary>
    /// Update of the whole Game
    /// Be careful when considering adding a Update method in another behaviour
    /// </summary>
    private void Update()
    {
        PlayerController.Singleton.Tick();
    }
}