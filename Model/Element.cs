using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Model
{
    public  class Element
    {
		public int Width { get; set; }
		public int Height { get; set; }

		public int X { get; set; }
		public int Y { get; set; }
        public int Z_Index { get; set; }
        public decimal Degree { get; set; }

        public decimal Opacity { get; set; }
	}
}
