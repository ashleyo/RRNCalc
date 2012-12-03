using System;
using System.Windows;
using System.Windows.Controls;
using CE = RPNCalc.CalcEngine;
namespace RPNCalc
{
    public partial class MainWindow : Window
    {
        private CalcEngine TheCalc = new CalcEngine();
        private CalcEngine KBBuffer = new CalcEngine();
        private Scale scale = new Scale();
        private bool FnState;

        public MainWindow()
        {
            InitializeComponent();

            B0.Tag = 0.0d;
            B1.Tag = 1.0d;
            B2.Tag = 2.0d;
            B3.Tag = 3.0d;
            B4.Tag = 4.0d;
            B5.Tag = 5.0d;
            B6.Tag = 6.0d;
            B7.Tag = 7.0d;
            B8.Tag = 8.0d;
            B9.Tag = 9.0d;
            BP.Tag = null;
            BE.Tag = null;
            Bchs.Tag = (CE.monop)((a) => -a);
            //note carefully order of operands in binary operations
            Badd.Tag = (CE.binop)((a, b) => b + a);
            Bmul.Tag = (CE.binop)((a, b) => b * a);
            Bdiv.Tag = (CE.binop)((a, b) => b / a);
            Bsub.Tag = (CE.binop)((a, b) => b - a);

            //For functions let's set labels too
            FnState = false;
            InvertFunctions();

            Bpi.Tag = Math.PI;
            Display.Text = "Ready";
        }

        private void InvertFunctions()
        {
            FnState = !FnState;
            if (FnState)
            {
                Bsin.Content = "sin";
                Bsin.Tag = (CE.monop)((a) => Math.Sin(a));
                Bcos.Content = "cos";
                Bcos.Tag = (CE.monop)((a) => Math.Cos(a));
                Btan.Content = "tan";
                Btan.Tag = (CE.monop)((a) => Math.Tan(a));
            }
            else
            {
                Bsin.Content = "asin";
                Bsin.Tag = (CE.monop)((a) => Math.Asin(a));
                Bcos.Content = "acos";
                Bcos.Tag = (CE.monop)((a) => Math.Acos(a));
                Btan.Content = "atan";
                Btan.Tag = (CE.monop)((a) => Math.Atan(a));
            }
        
        }

        private void ConGrpClick(object sender, RoutedEventArgs e)
        {
            KBBuffer.Commit((double)(sender as Button).Tag);
            EnterClick(sender, e);
        }

        private void NumGrpClick(object sender, RoutedEventArgs e)
        {
            double E = double.IsNaN(KBBuffer.ToS) ? 0d : KBBuffer.ToS;
           
            if (!scale.NZ) E *= 10;

            E += (double)((sender as Button).Tag) / scale.AntiLog;

            if (scale.NZ) scale.Inc();
            updateDisplay(E);
            KBBuffer.Commit(E);
        }

        private void OpGrpClick(object sender, RoutedEventArgs e)
        {
            EnterClick(sender, e);

            var v = (sender as Button).Tag;

            if (v is CE.binop) TheCalc.Reduce(v as CE.binop); else TheCalc.Reduce(v as CE.monop);

            Update();
        }

        private void CtlGrpClick(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Inv":
                    InvertFunctions();
                    break;
                case "CA":
                    TheCalc.Reset();
                    Update();
                    break;
                case "CE":
                    TheCalc.Reduce((a, b) => b);
                    Update();
                    break;
                case "DEL":
                    KBBuffer.Uncommit();
                    if (scale.NZ) scale.Dec();
                    updateDisplay(KBBuffer.ToS);
                    break;
                    
                default:
                    throw new NotImplementedException();
            }
        }

        private void EnterClick(object sender, RoutedEventArgs e)
        {
            if (!double.IsNaN(KBBuffer.ToS))
            {
                TheCalc.Commit(KBBuffer.ToS);
                Update();
            }
        }

        private void DPClick(object sender, RoutedEventArgs e)
        {
            if (!scale.NZ) scale.Inc();
        }

        private void Update()
        {
            updateDisplay();
            updateStackView();
            resetInput();
        }

        private void updateDisplay(double d)
        {
            Display.Text = d.ToString();
        }

        private void updateDisplay()
        {
            updateDisplay(TheCalc.ToS);
        }

        private void updateStackView()
        {
            StackView.Text = TheCalc.StackToString();
        }

        private void resetInput()
        {
            KBBuffer.Reset();
            scale.Reset();
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// A integral datatype with the following features: 
        /// i) it may be incremented, Inc, or decremented, Dec, but will always be non-negative (0.Dec => 0)
        /// ii) it may be forced to zero, Reset
        /// iii) you may test whether it is zero or not, NZ returns True if the object's value is non-zero
        /// iv) There is no method/property to obtain the evalue directly, but you can obtain its antilog (as a double), x.Antilog => 10**x 
        /// </remarks>
        private struct Scale
        {
            int s;
            public void Inc() { s++; }
            public void Dec() { if (s > 0) s--; }
            public void Reset() { s = 0; }
            public bool NZ { get { return s > 0; } }
            public double AntiLog { get { return Math.Pow(10.0d, s); } }
        }

    }
}
