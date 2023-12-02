using Godot;

public static class Random
{
    public static RandomNumberGenerator rng = new();

    public static void Randomize()
    {
        rng.Randomize();
    }
}
