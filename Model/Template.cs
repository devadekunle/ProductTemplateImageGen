using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Model
{
    public class Template
    {
        public List<TextElement> TextElements { get; set; }
        
        public List<ImageElement> ImageElements { get; set; }

        public List<ShapeElement> ShapeElements { get; set; }
        
        public int Width { get; set; }

        public int Height { get; set; }
    }
}
