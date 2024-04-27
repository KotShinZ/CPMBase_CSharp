using System.Numerics;
using Newtonsoft.Json;

namespace CPMBase;

/// <summary>
///  実際の位置と配列での位置を持つ
/// </summary>
[Serializable]
public class Position
{
    [JsonIgnore]
    public Vector3 position; //実際の位置
    public Vector3 arrayPosition; //配列での位置

    //[JsonIgnore]
    //public Vector3 dV = new Vector3(1, 1, 1);

    public Position(Vector3 arrayPosition, Vector3 dV, Range3 range)
    {
        this.arrayPosition = arrayPosition;
        this.position = new Vector3(
            (float)(range.x.min + dV.X * arrayPosition.X),
            (float)(range.y.min + dV.Y * arrayPosition.Y),
            (float)(range.z.min + dV.Z * arrayPosition.Z)
        );
        //this.dV = dV;
    }

    public Position(Vector3 arrayPosition)
    {
        this.arrayPosition = arrayPosition;
        this.position = arrayPosition;
    }

    public Position(float x, float y, float z)
    {
        this.arrayPosition = new Vector3(x, y, z);
        this.position = arrayPosition;
    }
}
