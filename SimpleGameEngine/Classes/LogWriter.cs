using System.IO;
using System.Text;
using System.Windows.Controls;

namespace SimpleGameEngine.Classes
{
    public class LogWriter: TextWriter
    {

        private readonly ItemsControl _outputControl;
            
        public LogWriter(ItemsControl listBox)
        {
            _outputControl = listBox;
        }
        
        public override void Write(string value)
        {
            _outputControl.Items.Add(value);
        }

        public override void WriteLine(string value)
        {
            _outputControl.Items.Add(value);
        }

        public override void Flush()
        {
            _outputControl.Items.Clear();
            base.Flush();
        }


        public override Encoding Encoding => Encoding.UTF8;
    }
}