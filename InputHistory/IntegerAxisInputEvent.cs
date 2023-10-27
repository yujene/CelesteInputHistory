using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.InputHistory
{
    public class IntegerAxisInputEvent : InputEvent
    {
        public int MoveX { get; }
        public int MoveY { get; }
        public int AimX { get; }
        public int AimY { get; }
        public int FeatherX { get; }
        public int FeatherY { get; }
        public bool MenuLeft { get; } = false;
        public bool MenuRight { get; } = false;
        public bool MenuUp { get; } = false;
        public bool MenuDown { get; } = false;

        public IntegerAxisInputEvent(VirtualIntegerAxis axisX, VirtualIntegerAxis axisY, VirtualJoystick aim, VirtualJoystick feather)
        {
            MoveX = (int)axisX;
            MoveY = (int)axisY;

            if (Math.Abs(aim.Value.X) < aim.Threshold)
                AimX = 0;
            else
                AimX = (int)(aim.Value.X / Math.Abs(aim.Value.X));
            if (Math.Abs(aim.Value.Y) < aim.Threshold)
                AimY = 0;
            else
                AimY = (int)(aim.Value.Y / Math.Abs(aim.Value.Y));

            if (Math.Abs(feather.Value.X) < feather.Threshold)
                FeatherX = 0;
            else
                FeatherX = (int)(feather.Value.X / Math.Abs(feather.Value.X));
            if (Math.Abs(feather.Value.Y) < feather.Threshold)
                FeatherY = 0;
            else
                FeatherY = (int)(feather.Value.Y / Math.Abs(feather.Value.Y));

            if (Engine.Scene is Level level)
            {
                // The frame you hit a button to close the pause menu, level.Pause becomes false,
                // so check wasPaused instead, as that stays true for one extra frame.
                if (level.Paused || DynamicData.For(level).Get<bool>("wasPaused"))
                {
                    MenuLeft = Input.MenuLeft;
                    MenuRight = Input.MenuRight;
                    MenuUp = Input.MenuUp;
                    MenuDown = Input.MenuDown;
                }
            }
        }

        public float Render(float x, float y, float fontSize)
        {
            var icon = Input.GuiDirection(new Vector2(MoveX, MoveY));
            icon?.Draw(new Vector2(x, y), Vector2.Zero, Color.White, fontSize / icon.Height);
            var rightDir = Input.GuiDirection(new Vector2(1, 0));
            return x + rightDir.Width * fontSize / rightDir.Height;
        }

        public bool Extends(InputEvent orig, bool tas)
        {
            if (orig is IntegerAxisInputEvent axisEvent)
            {
                return MoveX == axisEvent.MoveX && MoveY == axisEvent.MoveY &&
                    AimX == axisEvent.AimX && AimY == axisEvent.AimY &&
                    FeatherX == axisEvent.FeatherX && FeatherY == axisEvent.FeatherY &&
                    (!tas || (
                        MenuLeft == axisEvent.MenuLeft &&
                        MenuRight == axisEvent.MenuRight &&
                        MenuUp == axisEvent.MenuUp &&
                        MenuDown == axisEvent.MenuDown));
            }
            return false;
        }

        public string ToTasString()
        {
            var ret = "";
            if (MenuLeft || (MoveX == -1 && AimX == -1 && FeatherX == -1))
                ret += "L,";
            else
            {
                if (MoveX == -1 && FeatherX == -1)
                    ret += "ML,";
                if (AimX == -1)
                    ret += "AL,";
            }
            if (MenuRight || (MoveX == 1 && AimX == 1 && FeatherX == 1))
                ret += "R,";
            else
            {
                if (MoveX == 1 && FeatherX == 1)
                    ret += "MR,";
                if (AimX == 1)
                    ret += "AR,";
            }
            if (MenuUp || (MoveY == -1 && AimY == -1 && FeatherY == -1))
                ret += "U,";
            else
            {
                if (MoveY == -1 && FeatherY == -1)
                    ret += "MU,";
                if (AimY == -1)
                    ret += "AU,";
            }
            if (MenuDown || (MoveY == 1 && AimY == 1 && FeatherY == 1))
                ret += "D,";
            else
            {
                if (MoveY == 1 && FeatherY == 1)
                    ret += "MD,";
                if (AimY == 1)
                    ret += "AD,";
            }
            return ret.Trim(',');
        }
    }
}
