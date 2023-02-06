using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Downloader
{
    public class TextConsole
    {
        private RichTextBox richTextBox;
        private int maximumLines = 50;

        public TextConsole(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        public void WriteLine(string line)
        {
            //if there are no lines, dont add a newline
            if (richTextBox.Lines.Length == 0)
            {
                richTextBox.AppendText(line);
            }
            else {
                richTextBox.AppendText("\n" + line);
            }

            //make last line always visible by moving the selection to last line
            richTextBox.SelectionStart = richTextBox.Text.LastIndexOfAny(Environment.NewLine.ToCharArray()) + 1;
            richTextBox.ScrollToCaret();
        }

        public void UpdateLine(string line)
        {
            var lines = richTextBox.Lines;
            lines[lines.Length - 1] = line;
            richTextBox.Lines = lines;
        }

        public void RemoveLastLine()
        {
            string text = richTextBox.Text;

            int lastNewLinePos = text.LastIndexOf('\n');
            text.Remove(lastNewLinePos);
        }
    }
}
