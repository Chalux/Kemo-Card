using Godot;
using System.Collections.Generic;

namespace KemoCard.Pages
{
    internal partial class Bezier : Control
    {
        int arrow_num = 10;
        List<Sprite2D> sprites = new();
        [Export] Texture2D t_arrow_head;
        [Export] Texture2D t_arrow_body;
        Vector2 ctrl_a_pos = new();
        Vector2 ctrl_b_pos = new();

        private static Vector2 CalculateCubicBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 point = new Vector2(
                (float)(uuu * p0.X + 3 * uu * t * p1.X + 3 * u * tt * p2.X + ttt * p3.X),
                (float)(uuu * p0.Y + 3 * uu * t * p1.Y + 3 * u * tt * p2.Y + ttt * p3.Y)
            );

            return point;
        }

        public override void _Ready()
        {
            for (int i = 0; i < arrow_num - 1; i++)
            {
                Sprite2D sprite2D = new();
                AddChild(sprite2D);
                sprites.Add(sprite2D);
                sprite2D.Texture = t_arrow_body;
                float x = (float)(i / arrow_num * 1 + 0.2);
                sprite2D.Scale = new Vector2(x, x);
            }
            Sprite2D arrow = new();
            AddChild(arrow);
            sprites.Add(arrow);
            arrow.Texture = t_arrow_head;
            arrow.Scale = new Vector2(1.2f, 1.2f);
        }

        public void Reset(Vector2 start_Pos, Vector2 end_Pos)
        {
            Show();

            ctrl_a_pos.X = (float)(start_Pos.X + (start_Pos.X - end_Pos.X) * 0.1);
            ctrl_a_pos.Y = (float)(end_Pos.Y - (end_Pos.Y - start_Pos.Y) * 0.2);
            ctrl_b_pos.X = (float)(start_Pos.X - (start_Pos.X - end_Pos.X) * 0.3);
            ctrl_b_pos.Y = (float)(end_Pos.Y + (end_Pos.Y - start_Pos.Y) * 0.3);

            //for (int i = 0; i < arrow_num; i++)
            //{
            //    var t = (float)(i / (arrow_num - 1));
            //    var pos = (start_Pos * ((1 - t) * (1 - t) * (1 - t))) + (3 * ctrl_a_pos * t * (1 - t) * (1 - t)) + (3 * ctrl_b_pos * t * t * (1 - t)) + (end_Pos * t * t * t);
            //    var sprite = sprites[i];
            //    sprite.GlobalPosition = pos;
            //    float x = (float)(t + 0.2);
            //    sprite.Scale = new Vector2(x, x);
            //}

            for (int i = 0; i < sprites.Count - 1; i++)
            {
                float t = (float)(i + 1) / sprites.Count;
                Vector2 point = CalculateCubicBezierPoint(start_Pos, ctrl_a_pos, ctrl_b_pos, end_Pos, t);
                sprites[i].GlobalPosition = point;
                float x = (float)(t + 0.2);
                sprites[i].Scale = new Vector2(x, x);
            }

            Sprite2D arrow = sprites[^1];
            arrow.GlobalPosition = GetGlobalMousePosition();
            arrow.Scale = new(1.2f, 1.2f);

            UpdateAngle();
        }

        public void UpdateAngle()
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
            Sprite2D curr = sprites[^1];
            Sprite2D last = sprites[^2];
            Vector2 diff = curr.GlobalPosition - last.GlobalPosition;
            var a = Mathf.RadToDeg(diff.Angle()) + 90;
            curr.RotationDegrees = a;
        }
    }
}
