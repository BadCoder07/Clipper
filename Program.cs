using System;
using System.Windows.Forms;
using Clipper.Modern;

namespace Clipper
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // Para .NET 6 ou superior
            Application.Run(new MainForm());
        }
    }
}
