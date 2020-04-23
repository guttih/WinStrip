using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinStrip.FormUtilities
{
    public static class PromptDialog
    {

        /// <summary>
        /// Shows a prompt dialog asking the user to write some text
        /// </summary>
        /// <param name="text">Text to describe what you are asking for</param>
        /// <param name="caption">Title of the text dialog</param>
        /// <param name="initialText">Default text in the text box which the user can write to.</param>
        /// <param name="width">With of the dialog</param>
        /// <returns></returns>
        public static string ShowDialog(string text, string caption, string initialText="", int width = 500)
        {
            Form prompt = new Form()
            {
                Width = width,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Width = (width-50-50), Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = width-50-50 };
            Button confirmation = new Button() { Text = "OK", Left = textBox.Width+50-60, Width = 60, Top = 80, Enabled=false, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            textBox.Text= initialText;

            textBox.TextChanged += new EventHandler(textBoxNoSend_TextChanged);

            void textBoxNoSend_TextChanged(object sender, EventArgs e)
            {
                confirmation.Enabled = textBox.Text.Length > 0;
            }

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
