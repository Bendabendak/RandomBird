using UnityEngine;

namespace SadJam.Components
{
    public abstract class SaveResultOperator_Vector2 : StructCalculatorOperator<Vector2>
    {
        private Vector2 _lastResult;
        private Vector2 _lastFirst;
        private Vector2 _lastSecond;

        public virtual bool SkipNaN => true;

        public sealed override Vector2 Calculate(Vector2 first, Vector2 second)
        {
            if (_lastFirst == first && _lastSecond == second) return _lastResult;

            _lastFirst = first;
            _lastSecond = second;

            _lastResult = Result(first, second);

            if (SkipNaN)
            {
                return OperatorUtility_Vector2.SkipNaN(_lastResult, first, second);
            }

            return _lastResult;
        }

        protected abstract Vector2 Result(Vector2 first, Vector2 second);
    }
}
