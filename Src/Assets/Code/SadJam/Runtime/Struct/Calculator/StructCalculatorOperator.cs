namespace SadJam
{
    public abstract class StructCalculatorOperator<T> : StaticBehaviour where T : struct
    {
        public abstract string Symbol { get; }

        public abstract T Calculate(T first, T second);
    }
}
