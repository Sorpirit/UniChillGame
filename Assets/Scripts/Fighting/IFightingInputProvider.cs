namespace Fighting
{
    public interface IFightingInputProvider
    {
        int ActiveAttackIndex { get; }

        void ResetAttackIndex(int index);
    }
}