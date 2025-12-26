namespace Player.Remote
{
    public class RemotePlayer : APlayer
    {
        private void Update()
        {
            SnapshotMotor.Tick();
        }
    }
}
