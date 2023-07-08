using Manifold.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCube.GCI
{
    public abstract class GciWithUniqueID<TBinarySerializable> : Gci<TBinarySerializable>
        where TBinarySerializable : IBinarySerializable, IBinaryFileType, new()
    {
        public abstract ushort Unknown { get; }
        public abstract ushort[] UniqueIDs { get; }

        public void ValidateUniqueID()
        {
            // Validate header with regards to expected data
            bool isValidUniqueID = false;
            foreach (var uniqueID in UniqueIDs)
            {
                if (uniqueID == header.UniqueID)
                {
                    isValidUniqueID = true;
                    break;
                }
            }
            if (!isValidUniqueID)
            {
                string msg = $"{nameof(UniqueIDs)} did not match header value {header.UniqueID:x4}.";
                throw new InvalidGciException(msg);
            }
        }
    }
}
