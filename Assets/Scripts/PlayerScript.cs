public class PlayerScript
{
    public int playerId;
    public TankScript tank; // referencia al tanque en la escena
    public PlayerAction pendingAction;

    public PlayerScript(int id, TankScript assignedTank)
    {
        playerId = id;
        tank = assignedTank;
    }
}