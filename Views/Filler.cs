using System;
using Terminal.Gui;

namespace tempalted {
    class Filler : View {
		public Filler (Rect rect) : base (rect)
		{
		}

		public override void Redraw (Rect region)
		{
			Driver.SetAttribute (ColorScheme.Focus);
			var f = Frame;

			for (int y = 0; y < f.Width; y++) {
				Move (0, y);
				for (int x = 0; x < f.Height; x++) {
					Rune r;
					switch (x % 3) {
					case 0:
						r = '.';
						break;
					case 1:
						r = 'o';
						break;
					default:
						r = 'O';
						break;
					}
					Driver.AddRune (r);
				}
			}
		}
	}
}