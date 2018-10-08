using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kcs_screenshot
{
    public partial class MainForm : Form
    {
        private ScreenCapture _capture = new ScreenCapture();
        private Toast _toast;// = new Toast();
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var ret = _capture.Do();
            if (string.IsNullOrEmpty(ret))
            {
                //MessageBox.Show("キャプチャーに失敗しました");
                _toast = new Toast("キャプチャーに失敗しました", Windows.UI.Notifications.ToastTemplateType.ToastText01);
            }
            else
            {
                //MessageBox.Show(string.Format("{0}に保存しました", ret));
                _toast = new Toast(string.Format("{0}に保存しました", ret), Windows.UI.Notifications.ToastTemplateType.ToastText01);
            }
            _toast.Show();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            Func<int> proc = () =>
            {
                while ((MouseButtons & MouseButtons.Left) != MouseButtons.Left)
                {
                    System.Diagnostics.Debug.Print("Wait Click");
                }
                _capture.SelectCaptureWindow();

                return 0;
            };
            int ret = await Task.Run(proc);
        }
    }
}
