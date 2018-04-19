using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject FallingBlockPrefab;
    private Vector2 _screenHalfSizeInWorldUnits;
    private float _secondsBetweenSpawnTime = 1;
    private float _nextSpawnTime;

    private void Start ()
    {
        float screenHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        float screenHalfHeight = Camera.main.orthographicSize;

        _screenHalfSizeInWorldUnits = new Vector2(screenHalfWidth, screenHalfHeight);
    }

    private void Update ()
    {
        float now = Time.time;

        if (now > _nextSpawnTime)
        {
            _nextSpawnTime = now + _secondsBetweenSpawnTime;

            float randomX = Random.Range(-_screenHalfSizeInWorldUnits.x, _screenHalfSizeInWorldUnits.x);
            float y = _screenHalfSizeInWorldUnits.y;
            Vector2 spawnPosition = new Vector2(randomX, y);

            Instantiate(FallingBlockPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
