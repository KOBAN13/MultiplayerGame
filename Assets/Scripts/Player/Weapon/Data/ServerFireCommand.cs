namespace Player.Weapon.Data
{
    public struct ServerFireCommand
    {
        public int weaponId;
        public byte alpha;
        public long snapshotId;
        public ShotData shotData;
    }
}