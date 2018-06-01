using UnityEngine;

[CreateAssetMenu()]
public class NoiseData : UpdatableData
{
    public Noise.NormalizeMode NormalizeMode;
    public float Scale;
    public int Octave;
    [Range(0, 1)]
    public float Persistence;
    public float Lacunarity;
    public int Seed;
    public Vector2 Offset;

    protected override void OnValidate()
    {
        if (Octave < 1)
            Octave = 1;
        if (Lacunarity < 1)
            Lacunarity = 1;
        if (Scale < 0.001f)
            Scale = 0.001f;

        base.OnValidate();
    }
}
