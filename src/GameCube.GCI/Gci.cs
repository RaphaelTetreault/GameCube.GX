
namespace GameCube.GCI
{
    public class Gci : Gci<GciVoid>
    {
        public override ushort UniqueID => header.UniqueID;
    }
}
