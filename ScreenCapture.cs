using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace kcs_screenshot
{
    class ScreenCapture
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("User32.Dll")]
        private static extern int GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        private extern static IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private extern static IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        private extern static bool GetCursorPos(out POINT point);

        private IntPtr _targetWindow = IntPtr.Zero;

        /// <summary>
        /// キャプチャーするウィンドウの設定
        /// </summary>
        public bool SelectCaptureWindow()
        {
            POINT point;
            if (GetCursorPos(out point))
            {
                _targetWindow = WindowFromPoint(point);
                return true;
            }
            return false;
        }

        /// <summary>
        /// キャプチャーの実行
        /// </summary>
        public string Do(bool isPreview = false)
        {
            if (_targetWindow == IntPtr.Zero)
                return string.Empty;

            RECT r;
            IntPtr active = _targetWindow;

            GetWindowRect(active, out r);
            Rectangle rect = new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
            }

            Trimming(bmp);

            var saveFileName = DateTime.Now.ToString().Replace(" ", "").Replace('/','-').Replace(":", "") + ".png";

            if (!isPreview)
                bmp.Save(saveFileName, ImageFormat.Png);

            return saveFileName;
        }

        /// <summary>
        /// キャプチャー範囲のトリミング
        /// </summary>
        /// <param name="bmp"></param>
        private void Trimming(Bitmap bmp)
        {
            var centerY = bmp.Height >> 1;
            var targetColor = bmp.GetPixel(2, centerY);

            //左上を検索
            int y;
            for (y=centerY; y >= 0; --y)
            {
                var color = bmp.GetPixel(2, y);
                if (targetColor != color)
                {
                    //DMM GAMEの黒いところまで行った
                    break;
                }
            }
        }
    }
}
