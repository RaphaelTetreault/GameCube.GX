
namespace GameCube.GCI
{
    public class Gci : Gci<GciVoid>
    {
        public override ushort[] UniqueIDs => new ushort[] { header.UniqueID };
    }
}
