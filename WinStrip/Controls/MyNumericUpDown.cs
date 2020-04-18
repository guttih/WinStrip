using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinStrip.Controls
{
    class MyNumericUpDown : NumericUpDown
    {
        public MyNumericUpDown()
        {
            MouseEnter += MouseEnterHandler;
            MouseLeave += MouseLeaveHandler;
        }

        private void MouseEnterHandler(object sender, EventArgs e)
        {
            Debug.WriteLine("entered");
        }

        private void MouseLeaveHandler(object sender, EventArgs e)
        {
            Debug.WriteLine("Left");
        }
    }
}
