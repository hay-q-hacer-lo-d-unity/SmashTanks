using Tank;

namespace Manager
{
    public class PlayerScript
    {
        public readonly int PlayerId;
        public readonly TankScript Tank;
        public PlayerAction PendingAction;

        public PlayerScript(int id, TankScript assignedTank)
        {
            PlayerId = id;
            Tank = assignedTank;
        }
    }
}