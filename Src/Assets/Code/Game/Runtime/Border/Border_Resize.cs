using SadJam;
using UnityEngine;

namespace Game 
{
    public class Border_Resize : DynamicExecutor
    {
        public override ExecutorBehaviour Behaviour => new()
        {
            Type = ExecutorBehaviourType.OnlyExecutable,
            InGarbage = false,
            OnlyOnePerObject = false
        };

        public GameObject Top;
        public GameObject Bottom;
        public GameObject Left; 
        public GameObject Right;

        public GameObject LeftTopEdge;
        public GameObject RightTopEdge;
        public GameObject LeftBottomEdge;
        public GameObject RightBottomEdge;

        protected override void DynamicExecutor_OnExecute()
        {
            Vector2 cameraRightTop = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
            Vector2 cameraLeftDown = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));

            float cameraHeight = Camera.main.orthographicSize * 2.0f;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            Top.transform.localScale = new(cameraWidth, Top.transform.localScale.y, Top.transform.localScale.z);
            Top.transform.position = new(Top.transform.position.x, cameraRightTop.y + Top.transform.localScale.y / 2f, Top.transform.position.z);

            Bottom.transform.localScale = new(cameraWidth, Top.transform.localScale.y, Top.transform.localScale.z);
            Bottom.transform.position = new(Top.transform.position.x, cameraRightTop.y - cameraHeight - Top.transform.localScale.y / 2f, Top.transform.position.z);

            Left.transform.position = new(cameraLeftDown.x - Left.transform.localScale.x / 2f, cameraLeftDown.y + cameraHeight / 2f);
            Left.transform.localScale = new(Left.transform.localScale.x, cameraHeight, Left.transform.localScale.z);

            Right.transform.position = new(cameraRightTop.x + Right.transform.localScale.x / 2f, cameraRightTop.y - cameraHeight / 2f);
            Right.transform.localScale = new(Right.transform.localScale.x, cameraHeight, Right.transform.localScale.z);

            LeftTopEdge.transform.position = new(cameraLeftDown.x, cameraLeftDown.y + cameraHeight);
            RightTopEdge.transform.position = new(cameraRightTop.x, cameraRightTop.y);

            LeftBottomEdge.transform.position = new(cameraLeftDown.x, cameraLeftDown.y);
            RightBottomEdge.transform.position = new(cameraRightTop.x, cameraRightTop.y - cameraHeight);
        }
    }
}
