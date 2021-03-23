using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Model
{
    public class Template
    {

        public List<TextElement> TextElements { get; set; } = new List<TextElement>();

        public List<ImageElement> ImageElements { get; set; } = new List<ImageElement>();

        public List<ShapeElement> ShapeElements { get; set; } = new List<ShapeElement>();
        
        public int Width { get; set; }

        public int Height { get; set; }
    }
}
