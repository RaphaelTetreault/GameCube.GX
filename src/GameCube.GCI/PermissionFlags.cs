namespace GameCube.GCI
{
    [System.Flags]
    public enum PermissionFlags : byte
    {
        IsPublic = 1 << 2,
        NoCopy = 1 << 3,
        NoMove = 1 << 4,
    }
}
