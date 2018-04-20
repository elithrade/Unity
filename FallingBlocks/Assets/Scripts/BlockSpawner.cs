using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject FallingBlockPrefab;
    // The spawn size min max is represented by a vector2 x is the min and y is the max
    public Vector2 SpawnSizeMinMax;
    public Vector2 SecondsInSpawnTimeMinMax;
    public float SpawnAngleMax;

    private Vector2 _screenHalfSizeInWorldUnits;
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
        if (now <= _nextSpawnTime)
            return;

        // User linear interpolation to figure out the spawn time
        // base on the current difficulty, y is max and x is min i.e. 1 .. 0
        float currentDifficulty = Difficulty.GetDifficultyPercentage();
        float spawnTime = Mathf.Lerp(SecondsInSpawnTimeMinMax.y, SecondsInSpawnTimeMinMax.x, currentDifficulty);
        _nextSpawnTime = now + spawnTime;

        float spawnSize = Random.Range(SpawnSizeMinMax.x, SpawnSizeMinMax.y);

        float randomX = Random.Range(-_screenHalfSizeInWorldUnits.x, _screenHalfSizeInWorldUnits.x);
        // To make the block spawn outside of the screen y axis, plus the spawnSize
        // This takes into account of block rotation as well
        float y = _screenHalfSizeInWorldUnits.y + spawnSize;
        Vector2 spawnPosition = new Vector2(randomX, y);

        // Note that in 2D view, the rotation is around the z axis
        GameObject newBlock = Instantiate(
            FallingBlockPrefab,
            spawnPosition,
            Quaternion.Euler(Vector3.forward * Random.Range(-SpawnAngleMax, SpawnAngleMax)));

        // This applies tp both x and y axis
        newBlock.transform.localScale = Vector2.one * spawnSize;
    }
}
