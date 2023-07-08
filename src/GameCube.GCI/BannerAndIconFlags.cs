namespace GameCube.GCI
{
    [System.Flags]
    public enum BannerAndIconFlags : byte
    {
        NoBanner = 0,

        // Color information
        IndirectColorCI8 = 1 << 0,
        DirectColorRGB565   = 1 << 1,
        InvalidBanner = IndirectColorCI8 | DirectColorRGB565,

        // Animation flags
        // Unset is loop, set it ping-pong (AKA seesaw)
        AnimationLoop     = 0 << 2,
        AnimationPingPong = 1 << 2,
    }
}
