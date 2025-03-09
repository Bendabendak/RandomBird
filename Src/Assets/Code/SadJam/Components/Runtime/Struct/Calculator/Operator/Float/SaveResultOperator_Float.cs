namespace SadJam.Components
{
    public abstract class SaveResultOperator_Float : StructCalculatorOperator<float>
    {
        private float _lastResult;
        private float _lastFirst;
        private float _lastSecond;
    
        public sealed override float Calculate(float first, float second)
        {
            if (_lastFirst == first && _lastSecond == second) return _lastResult;
    
            _lastFirst = first;
            _lastSecond = second;
    
            _lastResult = Result(first, second);
    
            return _lastResult;
        }
    
        protected abstract float Result(float first, float second);
    }
}
