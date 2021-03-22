using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Model
{
    public class ImageElement : Element
    {
      public string ImageUrl { get; set; }
      public bool IsFlipped { get; set; }

    }
}
