using UnityEngine;

namespace SadJam.Components
{
    public abstract class SaveResultOperator_Vector3 : StructCalculatorOperator<Vector3>
    {
        private Vector3 _lastResult;
        private Vector3 _lastFirst;
        private Vector3 _lastSecond;

        public sealed override Vector3 Calculate(Vector3 first, Vector3 second)
        {
            if (_lastFirst == first && _lastSecond == second) return _lastResult;
    
            _lastFirst = first;
            _lastSecond = second;
    
            _lastResult = Result(first, second);
    
            return _lastResult;
        }
    
        protected abstract Vector3 Result(Vector3 first, Vector3 second);
    }
}
