using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Messenger.Client
{
    public class UIHelper
    {
        public static void SetRoundedRegion(Control ctrl, int radius)
        {
            using (var path = new GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
                path.CloseFigure();
                ctrl.Region = new Region(path);
            }
        }

        public static void DrawAvatar(Graphics g, Rectangle rect, string text, Color? backColor = null)
        {
            if (backColor == null)
                backColor = Color.FromArgb(63, 81, 181);

            // Рисуем круг
            using (var path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (var brush = new SolidBrush(backColor.Value))
                    g.FillPath(brush, path);
            }

            if (string.IsNullOrEmpty(text))
                text = "?";

            // Подбираем размер шрифта, чтобы текст помещался в круге с отступом 4px
            int maxSize = Math.Min(rect.Width, rect.Height) - 8;
            float fontSize = 16f;
            using (var testFont = new Font("Segoe UI", fontSize, FontStyle.Bold))
            {
                SizeF textSize = g.MeasureString(text, testFont);
                while ((textSize.Width > maxSize || textSize.Height > maxSize) && fontSize > 8)
                {
                    fontSize -= 1f;
                    testFont.Dispose();
                    using (var newFont = new Font("Segoe UI", fontSize, FontStyle.Bold))
                        textSize = g.MeasureString(text, newFont);
                }
            }

            using (var font = new Font("Segoe UI", fontSize, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(text, font, Brushes.White, rect, sf);
            }
        }
    }
}
