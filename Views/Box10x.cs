using System;
using Terminal.Gui;

namespace tempalted {
    class Box10x : View {
        public Box10x (int x, int y) : base (new Rect (x, y, 10, 10))
        {
        }

        public override void Redraw (Rect region)
        {
            Driver.SetAttribute (ColorScheme.Focus);

            for (int y = 0; y < 10; y++) {
                Move (0, y);
                for (int x = 0; x < 10; x++) {
                    Driver.AddRune ((Rune)('0' + (x + y) % 10));
                }
            }

        }
    }
}