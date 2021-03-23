using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductTemplateImageGen.Model;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ProductTemplateImageGen.Service
{
	public class ImageGenerator : IImageGenerator
	{
		private readonly Product product;
		private readonly Template template;
		private static HttpWebRequest webClient;
		public ImageGenerator()
		{
			product = new Product
			{
				Currency = "USD",
				Name = "Headphone",
				Price = 50,
				ImageUrl = "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?ixid=MXwxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHw%3D&ixlib=rb-1.2.1&auto=format&fit=crop&w=1920&q=1280"
			};

			template = new Template
			{
				ImageElements = new List<ImageElement>() {
					new ImageElement { Y = 0, X = 0, ImageUrl = "https://thumbs.dreamstime.com/b/discount-stamp-vector-clip-art-33305813.jpg" }
				},
				TextElements = new List<TextElement>
					 {
						  new TextElement
						  {
							Height = 300, Width = 600, FontSize = 50, FontWeight = 3, Z_Index = 1, IsItalic = false, isBold = true, Opacity = 1, X = 100, Y = 1200,Color = "Red", FontFamily="Arial", isVertical = true,
							  Text = @"{Name} is selling at {Price}{Currency}. It has been the industry's standard dummy text ever since the 1500s,when an unknown printer took a of type and scrambled it to make a type specimen book"
						  },
					new TextElement
						  {
								Height = 100, Width = 400, FontSize = 75, FontWeight = 3, Z_Index = 1, IsItalic = true, isUnderLine = true, isBold = true, Opacity = 1, X = 100, Y = 1160, Color = "Blue", BackgroundColor = "Yellow",FontFamily="Calibri",
							  Text = "{Name} is the best",
						  }
},
				Height = 1024,
				Width = 1024
				
			};


		}


		public async Task GenerateProductTemplate()
		{
			var backgroundImage = DownloadImageFromUrl(product.ImageUrl);
			var canvas = await LoadImage(backgroundImage);
			if (template.ImageElements.Count > 0)
			{
				canvas = await AddImage(canvas, template);
			}

			if (template.TextElements.Count > 0)
			{
				//var processedTextElements = GenerateInterpolatedText(template.TextElements, product);
				//canvas = await LayerTexts(canvas, processedTextElements);

				var processedTextElements = LayerTexts(canvas, template.TextElements, product);
			}

			await SaveImage(canvas, "newImage.png");
		}

		private Task<GcBitmap> LoadImage(Image image)
		{
			var bmp = new GcBitmap(image);

			//Load image
			return Task.FromResult(bmp);
		}

		/// <summary>
		/// dynamic text
		/// tex1: long text, on 3 lines, red, font = arial black, font size = 13, vertical, opacity = 50%
		/// text2: short text, blue with yellow backgrund, font = calibri, bold, underlined, italic, 30deg, font size = 20, opacity = 75%
		/// text2 is above text1, they overlap 30% (partially)
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="textElements"></param>
		/// <param name="product"></param>
		/// <returns></returns>
		private Task<GcBitmap> LayerTexts(GcBitmap bmp, List<TextElement> textElements, Product product)
		{
			var g = bmp.CreateGraphics();
			var jsonStringProduct = JsonConvert.SerializeObject(product);

			foreach (var item in textElements)
			{

				TextLayout tl = g.CreateTextLayout();
				tl.DefaultFormat.FontSize = item.FontSize;
				tl.DefaultFormat.FontBold = item.isBold;
				tl.DefaultFormat.FontItalic = item.IsItalic;
				tl.DefaultFormat.UprightInVerticalText = item.isVertical;
				tl.MaxWidth = item.Width;
				
				tl.MaxHeight = item.Height;
				tl.WrapMode = WrapMode.WordWrap;
				
				tl.DefaultFormat.ForeColor = Color.FromName(item.Color);
				if (!string.IsNullOrEmpty(item.BackgroundColor))
				{
					tl.DefaultFormat.BackColor = Color.FromName(item.BackgroundColor);
				}
				tl.DefaultFormat.Underline = item.isUnderLine;
				tl.DefaultFormat.Font = Font.FromFile(Path.Combine("Resources", "Fonts", $"{ item.FontFamily }.ttf"));
				tl.Append(BuildDynamicText(jsonStringProduct, item.Text));
				tl.TextAlignment = TextAlignment.Justified;
				g.DrawTextLayout(tl, new PointF(item.X, item.Y));
				
			}
			//Save the image rendering different shapes
			return Task.FromResult(bmp);
		}

		private string BuildDynamicText(string jsonStringProduct, string text)
		{
			var tokens = new Dictionary<string,string>();
			var product = JObject.Parse(jsonStringProduct);
			Regex reg = new Regex(@"{\w+}");
			foreach (Match match in reg.Matches(text))
			{
				var propertyName = match.Value.Substring(1, match.Value.Length-2);
				tokens.Add(match.Value, product.Value<string>(propertyName));
			}

			foreach (var token in tokens)
			{
				text = text.Replace(token.Key, tokens[token.Key]);
			}
			return text;
		}

		private Task<GcBitmap> AddImage(GcBitmap bmp, Template template)
		{

			foreach (var item in template.ImageElements)
			{
				var image = DownloadImageFromUrl(item.ImageUrl);

				GcBitmap addedimage = new GcBitmap(image);
				//Combine the two images using various compositing and blending modes
				bmp.CompositeAndBlend(addedimage, item.X, item.Y, CompositeMode.SourceOver, BlendMode.Normal);

			}
			return Task.FromResult(bmp);
		}

		private static Image DownloadImageFromUrl(string item)
		{
			webClient = (HttpWebRequest)HttpWebRequest.Create(item);
			webClient.AllowWriteStreamBuffering = true;
			webClient.AllowReadStreamBuffering = true;
			webClient.Timeout = 30000;

			var webResponse = webClient.GetResponse();
			var stream = webResponse.GetResponseStream();
			var image = Image.FromStream(stream);


			return image;
		}

		private Task SaveImage(GcBitmap bmp, string fileName)
		{
			bmp.SaveAsJpeg(fileName);
			return Task.CompletedTask;
		}


	}
}
