using BBI.Core.IO.Streams;
using BBI.Core.Utility.FixedPoint;
using BBI.Game.Data;
using System.IO;

namespace Subsystem
{
    public class NavMeshAttributesWrapper : NavMeshAttributes
    {
        private float distanceFromObstacles;
        private float distanceErrorPercentageTolerance;

        public NavMeshAttributesWrapper(NavMeshAttributes other)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryStreamWriter(stream);
                var reader = new BinaryStreamReader(stream);

                other.Write(writer);
                Read(reader);
            }

            BlockedBy = other.BlockedBy;
            UniqueTypeName = other.UniqueTypeName;
        }

        public Fixed64 DistanceFromObstacles
        {
            get { return Fixed64.FromConstFloat(distanceFromObstacles); }
            set { distanceFromObstacles = Fixed64.UnsafeFloatValue(value); }
        }

        public Fixed64 DistanceErrorPercentageTolerance
        {
            get { return Fixed64.FromConstFloat(distanceErrorPercentageTolerance); }
            set { distanceErrorPercentageTolerance = Fixed64.UnsafeFloatValue(value); }
        }

        public UnitClass BlockedBy { get; set; }

        public string UniqueTypeName { get; private set; }

        public bool Equals(NavMeshAttributes other)
        {
            var copy = new NavMeshAttributesWrapper(other);
            
            return distanceFromObstacles == copy.distanceFromObstacles
                && distanceErrorPercentageTolerance == copy.distanceErrorPercentageTolerance
                && BlockedBy == copy.BlockedBy;
        }

        public void Read(BinaryStreamReader reader)
        {
            distanceFromObstacles = reader.ReadSingle();
            distanceErrorPercentageTolerance = reader.ReadSingle();
            BlockedBy = (UnitClass)reader.ReadInt32();
        }

        public void Write(BinaryStreamWriter writer)
        {
            writer.WriteSingle(distanceFromObstacles);
            writer.WriteSingle(distanceErrorPercentageTolerance);
            writer.WriteInt32((int)BlockedBy);
        }
    }
}
