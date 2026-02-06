using UnityEngine;

/// <summary>
/// MonoSingleton is the abstract class for every Singleton in the Game
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Singleton { get; private set; }

    protected virtual void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            DestroyImmediate(gameObject);
            throw new System.Exception("Tried to create a Singleton twice");
        }
        else
        {
            Singleton = this as T;
        }
    }
}