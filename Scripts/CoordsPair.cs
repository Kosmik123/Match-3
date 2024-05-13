using UnityEngine;

namespace Bipolar.Match3
{
    public readonly struct CoordsPair : System.IEquatable<CoordsPair>
    {
        public readonly Vector2Int firstCoord;
        public readonly Vector2Int secondCoord;

        public Vector2Int this[int index] => index switch
        {
            0 => firstCoord,
            1 => secondCoord,
            _ => throw new System.IndexOutOfRangeException(),
        };

        public CoordsPair(Vector2Int coord1, Vector2Int coord2)
        {
            (firstCoord, secondCoord) = GetSortedCoords(coord1, coord2);
        }

        private static (Vector2Int smaller, Vector2Int larger) GetSortedCoords(Vector2Int coord1, Vector2Int coord2)
        {
            if (coord2.y < coord1.y)
                return (coord2, coord1);

            if (coord1.y < coord2.y)
                return (coord1, coord2);    
            
            if (coord2.x < coord1.x)
                return (coord2, coord1);
                
            return (coord1, coord2);
        }

        public override int GetHashCode()
        {
            return firstCoord.GetHashCode() ^ secondCoord.GetHashCode();
        }

        public bool Equals(CoordsPair other)
        {
            return firstCoord.Equals(other.firstCoord) && secondCoord.Equals(other.secondCoord);
        }
    }
}
