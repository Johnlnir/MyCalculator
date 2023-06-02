using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyCalculator
{
    class Variable
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public int Tag { get; set; }
        public String LastValue { get; set; }
        public bool enable { get; set; }
    }
}
