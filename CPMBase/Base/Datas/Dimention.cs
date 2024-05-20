namespace CPMBase;

using System.Numerics;

public enum Dimention
{
    _0d = 0,
    _1d = 1,
    _2d = 2,
    _3d = 3
}

public static class DimentionExpand
{
    public static Vector3 GetUnitVec(this Dimention dim)
    {
        switch (dim)
        {
            case Dimention._0d:
                return new Vector3(0, 0, 0);
            case Dimention._1d:
                return new Vector3(1, 0, 0);
            case Dimention._2d:
                return new Vector3(1, 1, 0);
            case Dimention._3d:
                return new Vector3(1, 1, 1);
            default:
                return new Vector3(0, 0, 0);
        }
    }

    public static List<Vector3> GetNextVecs(this Dimention dim)
    {
        var vecs = new List<Vector3>();
        switch (dim)
        {
            case Dimention._0d:
                break;
            case Dimention._1d:
                vecs.Add(new Vector3(1, 0, 0));
                vecs.Add(new Vector3(-1, 0, 0));
                break;
            case Dimention._2d:
                vecs.Add(new Vector3(1, 0, 0));
                vecs.Add(new Vector3(-1, 0, 0));
                vecs.Add(new Vector3(0, 1, 0));
                vecs.Add(new Vector3(0, -1, 0));
                break;
            case Dimention._3d:
                vecs.Add(new Vector3(1, 0, 0));
                vecs.Add(new Vector3(-1, 0, 0));
                vecs.Add(new Vector3(0, 1, 0));
                vecs.Add(new Vector3(0, -1, 0));
                vecs.Add(new Vector3(0, 0, 1));
                vecs.Add(new Vector3(0, 0, -1));
                break;
        }
        return vecs;
    }
}
