namespace Movement
{
    public interface IMovementInputProvider
    {
        MovementInput Input { get; }

        void ResetOneTimeActions();
    }
}