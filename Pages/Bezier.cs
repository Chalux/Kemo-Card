using System.Collections.Generic;
using Godot;

namespace KemoCard.Pages
{
    internal partial class Bezier : Control
    {
        private int _arrowNum = 10;
        private readonly List<Sprite2D> _sprites = [];
        [Export] private Texture2D _tArrowHead;
        [Export] private Texture2D _tArrowBody;
        private Vector2 _ctrlAPos;
        private Vector2 _ctrlBPos;

        private static Vector2 CalculateCubicBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            var point = new Vector2(
                uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X,
                uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y
            );

            return point;
        }

        private static Vector2 AutoCalculateBezier(Vector2 startPos, Vector2 endPos, float t)
        {
            var tempPos1 = new Vector2();
            var tempPos2 = new Vector2();
            tempPos1.X = (float)(startPos.X + (startPos.X - endPos.X) * 0.1);
            tempPos1.Y = (float)(endPos.Y - (endPos.Y - startPos.Y) * 0.2);
            tempPos2.X = (float)(startPos.X - (startPos.X - endPos.X) * 0.3);
            tempPos2.Y = (float)(endPos.Y + (endPos.Y - startPos.Y) * 0.3);
            return CalculateCubicBezierPoint(startPos, tempPos1, tempPos2, endPos, t);
        }

        public override void _Ready()
        {
            for (var i = 0; i < _arrowNum - 1; i++)
            {
                Sprite2D sprite2D = new();
                AddChild(sprite2D);
                _sprites.Add(sprite2D);
                sprite2D.Texture = _tArrowBody;
                var x = (float)(i / (float)_arrowNum * 1 + 0.2);
                sprite2D.Scale = new Vector2(x, x);
            }

            Sprite2D arrow = new();
            AddChild(arrow);
            _sprites.Add(arrow);
            arrow.Texture = _tArrowHead;
            arrow.Scale = new Vector2(1.2f, 1.2f);
        }

        public void Reset(Vector2 startPos, Vector2 endPos)
        {
            Show();

            _ctrlAPos.X = (float)(startPos.X + (startPos.X - endPos.X) * 0.1);
            _ctrlAPos.Y = (float)(endPos.Y - (endPos.Y - startPos.Y) * 0.2);
            _ctrlBPos.X = (float)(startPos.X - (startPos.X - endPos.X) * 0.3);
            _ctrlBPos.Y = (float)(endPos.Y + (endPos.Y - startPos.Y) * 0.3);

            //for (int i = 0; i < arrow_num; i++)
            //{
            //    var t = (float)(i / (arrow_num - 1));
            //    var pos = (start_Pos * ((1 - t) * (1 - t) * (1 - t))) + (3 * ctrl_a_pos * t * (1 - t) * (1 - t)) + (3 * ctrl_b_pos * t * t * (1 - t)) + (end_Pos * t * t * t);
            //    var sprite = sprites[i];
            //    sprite.GlobalPosition = pos;
            //    float x = (float)(t + 0.2);
            //    sprite.Scale = new Vector2(x, x);
            //}

            for (var i = 0; i < _sprites.Count - 1; i++)
            {
                var t = (float)(i + 1) / _sprites.Count;
                var point = CalculateCubicBezierPoint(startPos, _ctrlAPos, _ctrlBPos, endPos, t);
                _sprites[i].GlobalPosition = point;
                var x = (float)(t + 0.2);
                _sprites[i].Scale = new Vector2(x, x);
            }

            var arrow = _sprites[^1];
            arrow.GlobalPosition = GetGlobalMousePosition();
            arrow.Scale = new(1.2f, 1.2f);

            UpdateAngle();
        }

        private void UpdateAngle()
        {
            //for (int i = 0; i < arrow_num; i++)
            //{
            //    if (i == 0)
            //    {
            //        sprites[0].RotationDegrees = 0;
            //    }
            //    else
            //    {
            //        Sprite2D curr = sprites[i];
            //        Sprite2D last = sprites[i - 1];
            //        Vector2 diff = curr.GlobalPosition - last.GlobalPosition;
            //        var a = Mathf.RadToDeg(diff.Angle()) + 90;
            //        curr.RotationDegrees = a;
            //    }
            //}
            var curr = _sprites[^1];
            var last = _sprites[^2];
            var diff = curr.GlobalPosition - last.GlobalPosition;
            var a = Mathf.RadToDeg(diff.Angle()) + 90;
            curr.RotationDegrees = a;
        }
    }
}