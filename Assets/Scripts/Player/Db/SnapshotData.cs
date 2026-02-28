namespace Player.Db
{
    public struct SnapshotData
    {
        public AimData AimData;
        public PlayerStateData PlayerState;
        public float Rotation;
        
        public long SnapshotId;
        public long LastProcessedInputSequence;
        public float ServerTime;
    }
}
