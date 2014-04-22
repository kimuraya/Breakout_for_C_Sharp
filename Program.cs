using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DxLibDLL;

namespace Breakout_for_C_Sharp
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form = new Form1();
            form.Show();

            while (form.Created)
            {
                form.MainLoop();
            }
        }
    }
}
