using System.Collections.Generic;

public static class Universe
{
    public const float gravitationalConstant = 0.00001f;
    public const float physicsTimeStep = 0.01f;
    public static List<CelestialBody> allCelestialBodies = new List<CelestialBody>();
}
