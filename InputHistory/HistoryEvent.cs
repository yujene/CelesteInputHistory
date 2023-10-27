﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.InputHistory
{
    public class HistoryEvent
    {
        public float Time { get; set; }
        public long Frames { get; set; }

        private readonly IEnumerable<InputEvent> _events;

        private HistoryEvent(IEnumerable<InputEvent> events)
        {
            Time = Engine.RawDeltaTime;
            Frames = 1;
            _events = events;
        }

        public static HistoryEvent CreateDefaultHistoryEvent(InputStates inputStates)
        {
            return new HistoryEvent(DefaultEventList(inputStates));
        }

        public static HistoryEvent CreateTasHistoryEvent(InputStates inputStates)
        {
            var events = DefaultEventList(inputStates);
            events.Add(new ButtonInputEvent(Input.Pause, Microsoft.Xna.Framework.Input.Keys.S, inputStates));
            events.Add(new ButtonInputEvent(Input.QuickRestart, Microsoft.Xna.Framework.Input.Keys.R, inputStates));
            events.Add(new ButtonInputEvent(Input.MenuJournal, Microsoft.Xna.Framework.Input.Keys.N, inputStates));
            events.Add(new ButtonInputEvent(Input.MenuConfirm, Microsoft.Xna.Framework.Input.Keys.O, inputStates));
            return new HistoryEvent(events);
        }

        private static List<InputEvent> DefaultEventList(InputStates inputStates)
        {
            var events = new List<InputEvent>();
            events.Add(new IntegerAxisInputEvent(Input.MoveX, Input.MoveY, Input.Aim, Input.Feather));
            events.Add(new ButtonInputEvent(Input.Jump, Microsoft.Xna.Framework.Input.Keys.J, inputStates));
            events.Add(new MultiButtonInputEvent(new[] {
                new ButtonInputEvent(Input.Dash, Microsoft.Xna.Framework.Input.Keys.X, inputStates),
                new ButtonInputEvent(Input.CrouchDash, Microsoft.Xna.Framework.Input.Keys.Z, inputStates),
            }));
            events.Add(new ButtonInputEvent(Input.Grab, Microsoft.Xna.Framework.Input.Keys.G, inputStates));
            return events;
        }

        public float Render(float y)
        {
            var scale = 0.5f;
            var fontSize = ActiveFont.LineHeight * scale;
            var x = 10f;

            foreach (var e in _events)
            {
                x = e.Render(x, y, fontSize);
            }

            ActiveFont.DrawOutline(Frames.ToString(),
                    position: new Vector2(x, y),
                    justify: new Vector2(0f, 0f),
                    scale: Vector2.One * scale,
                    color: Color.White, stroke: 2f,
                    strokeColor: Color.Black);
            return y + fontSize;
        }

        public bool Extends(HistoryEvent other, bool tas)
        {
            if (other == null) return false;

            if (_events.Count() != other._events.Count()) return false;
            return _events.Zip(other._events, Tuple.Create)
                .All((es) => es.Item1.Extends(es.Item2, tas));
        }

        public string ToTasString()
        {
            string ret = Frames.ToString();
            foreach (var e in _events)
            {
                string s = e.ToTasString();
                if (s != "") ret += "," + s;
            }
            return ret;
        }
    }
}
