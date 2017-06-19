using UnityEngine;
using System.Collections;

public interface CollisionableToCharacter
{
    void CollisionAction(Character2 chara, Collision c);
}

public static class StaticParameter
{
    public static float FALL = 50;
    public static float FALL_NUTRAL = -25;
    public static float FLOOR_HEIGHT = 25;
    public static float DISAPPEAR_TIME = 2f;
    public static float FALL_DAMAGE = 3000f;
}