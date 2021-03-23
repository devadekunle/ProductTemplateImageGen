using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Model
{
    public class Product
    {
		public string Name { get; set; }
		public string Currency { get; set; }
		public decimal Price { get; set; }
		public string ImageUrl { get; set; }
	}
}
