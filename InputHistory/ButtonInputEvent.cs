using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.InputHistory
{
    public class ButtonInputEvent : InputEvent
    {
        public int Check { get; }
        public bool Pressed { get; }
        private readonly VirtualButton _button;
        private readonly List<int> _checkedBindingIds;
        private InputStates _inputStates;
        public Microsoft.Xna.Framework.Input.Keys Key { get; }

        public ButtonInputEvent(VirtualButton button, Microsoft.Xna.Framework.Input.Keys key, InputStates inputStates)
        {
            _button = button;
            Key = key;
            _inputStates = inputStates;
            _checkedBindingIds = CheckCount(button);
            Check = _checkedBindingIds.Count();
            Pressed = button.Binding.Pressed(button.GamepadIndex, button.Threshold);
        }

        public float Render(float x, float y, float fontSize)
        {
            var icon = Input.GuiKey(Key);
            float multiScale = 5f / (5 + Check - 1);
            for (int i = 0; i < Check; i++)
            {
                var shift = new Vector2(icon.Width * fontSize / icon.Height * (i / (5f + Check - 1)),
                    fontSize * ((Check - i - 1) / (5f + Check - 1)));
                icon.Draw(new Vector2(x, y) + shift, Vector2.Zero, Color.White, fontSize / icon.Height * multiScale);
            }
            return x + icon.Width * fontSize / icon.Height;
        }
        public bool Extends(InputEvent orig, bool tas)
        {
            if (orig is ButtonInputEvent origEvent)
            {
                return !Pressed && Check == origEvent.Check;
            }
            return false;
        }

        public override string ToString()
        {
            if (Check == 0) return "";

            var ret = "";
            if (_button == Input.Jump) ret += "Jump";
            else if (_button == Input.Dash) ret += "Dash";
            else if (_button == Input.CrouchDash) ret += "CrouchDash";
            else if (_button == Input.Grab) ret += "Grab";
            else if (_button == Input.Pause) ret += "Pause";
            else if (_button == Input.QuickRestart) ret += "QuickRestart";
            else if (_button == Input.MenuJournal) ret += "Journal";
            else if (_button == Input.MenuConfirm) ret += "Confirm";
            else ret += "Unknown";
            ret += " " + Check.ToString();
            if (Pressed) ret += "P";
            return ret;
        }

        public string ToTasString()
        {
            if (Check == 0) return "";

            var ret = "";
            if (_button == Input.Jump)
            {
                if (_inputStates.Jump == 1) ret += "J";
                else if (_inputStates.Jump == 2) ret += "K";
            }
            else if (_button == Input.Dash)
            {
                if (_inputStates.Dash == 1) ret += "C";
                else if (_inputStates.Dash == 2) ret += "X";
            }
            else if (_button == Input.CrouchDash)
            {
                if (_inputStates.Demo == 1) ret += "Z";
                else if (_inputStates.Demo == 2) ret += "V";
            }
            else if (_button == Input.Grab) ret += "G";
            else if (_button == Input.Pause) ret += "S";
            else if (_button == Input.QuickRestart) ret += "Q";
            else if (_button == Input.MenuJournal) ret += "N";
            else if (_button == Input.MenuConfirm) ret += "O";
            return ret;
        }

        private static List<int> CheckCount(VirtualButton button)
        {
            var ret = new List<int>();
            int idx = 0;
            foreach (var key in button.Binding.Keyboard)
            {
                if (MInput.Keyboard.Check(key)) ret.Add(idx);
                idx++;
            }
            // Hack to deal with Pause and ESC being different.
            if (button == Input.Pause && Input.ESC.Check) ret.Add(idx);

            idx = 100;
            foreach (var padButton in button.Binding.Controller)
            {
                if (MInput.GamePads[button.GamepadIndex].Check(padButton, button.Threshold))
                    ret.Add(idx);
                idx++;
            }
            return ret;
        }

    }
}
