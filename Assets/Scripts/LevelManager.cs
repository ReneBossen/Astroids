using Assets.Scripts;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public int CurrentLevel { get; private set; } = 0;
    private int _astroidsToSpawn;
    private int _astroidsRemaining;

    public delegate void LevelStarted(int level);
    public static event LevelStarted OnLevelStarted;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        _astroidsToSpawn = 2;
    }

    public void StartGame()
    {
        CurrentLevel = 1;
        StartLevel();
    }

    private void StartLevel()
    {
        _astroidsRemaining = _astroidsToSpawn * CurrentLevel;
        AstroidManager.Instance.SpawnAstroids(_astroidsRemaining);
        OnLevelStarted?.Invoke(CurrentLevel);
    }

    public void AsteroidDestroyed()
    {
        _astroidsRemaining--;

        if (_astroidsRemaining <= 0)
        {
            NextLevel();
        }
    }

    private void NextLevel()
    {
        CurrentLevel++;
        StartLevel();
    }
}