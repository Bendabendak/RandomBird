using UnityEngine;

namespace SadJam.Components 
{
    public abstract class SaveResultOperator_Quaternion : StructCalculatorOperator<Quaternion>
    {
        private Quaternion _lastResult;
        private Quaternion _lastFirst;
        private Quaternion _lastSecond;

        public sealed override Quaternion Calculate(Quaternion first, Quaternion second)
        {
            if (_lastFirst == first && _lastSecond == second) return _lastResult;

            _lastFirst = first;
            _lastSecond = second;

            _lastResult = Result(first, second);

            return _lastResult;
        }

        protected abstract Quaternion Result(Quaternion first, Quaternion second);
    }
}
