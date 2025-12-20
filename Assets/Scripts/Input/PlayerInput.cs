namespace Input
{
    public struct PlayerInputData
    {
        public readonly float Horizontal;
        public readonly float Vertical;
        public readonly bool IsJumping;
        public readonly bool IsRunning;
        public readonly bool IsOnGround;
        
        public PlayerInputData(float horizontal, float vertical, bool isJumping, bool isRunning, bool isOnGround)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            IsJumping = isJumping;
            IsRunning = isRunning;
            IsOnGround = isOnGround;
        }
    }
}