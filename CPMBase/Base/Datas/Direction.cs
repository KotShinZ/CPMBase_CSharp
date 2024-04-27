using System.Numerics;

namespace CPMBase.Base
{
    public enum Direction
    {
        xPlus,
        xMinus,
        yPlus,
        yMinus,
        zPlus,
        zMinus
    }

    public class DirectionHelper
    {
        public static Vector3 GetVector(Direction dir)
        {
            switch (dir)
            {
                case Direction.xPlus:
                    return new Vector3(1, 0, 0);
                case Direction.xMinus:
                    return new Vector3(-1, 0, 0);
                case Direction.yPlus:
                    return new Vector3(0, 1, 0);
                case Direction.yMinus:
                    return new Vector3(0, -1, 0);
                case Direction.zPlus:
                    return new Vector3(0, 0, 1);
                case Direction.zMinus:
                    return new Vector3(0, 0, -1);
                default:
                    return new Vector3(0, 0, 0);
            }
        }
    }
}