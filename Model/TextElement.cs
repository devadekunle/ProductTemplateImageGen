namespace ProductTemplateImageGen.Model
{
	public class TextElement : Element
	{
		public string Text { get; set; }
		public int FontSize { get; set; }

		public int FontFamily { get; set; }

		public decimal Opacity { get; set; }
		
		public int FontWeight { get; set; }

		public bool isBold { get; set; }

		public bool IsItalic { get; set; }

		public decimal Degree { get; set; }


	}
}
