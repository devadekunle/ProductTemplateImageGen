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
					new ImageElement { Y = 0, X = 0, Height = 200, Width = 200, ImageUrl = "https://thumbs.dreamstime.com/b/discount-stamp-vector-clip-art-33305813.jpg" }
				},
				TextElements = new List<TextElement>
					 {
						  new TextElement
						  {
							Height = 100, Width = 500, FontSize = 25, FontWeight = 3, Z_Index = 1, IsItalic = false, isBold = true, Opacity = 1, X = 50, Y = 800,Color = "Red", FontFamily="Arial",
							  Text = @"{Name} is sold at {Price}{Currency}. IT has been the industry's standard dummy text ever since the 1500s,when an unknown printer took a of type and scrambled it to make a type specimen book"
						  }, 
					new TextElement
						  {
								Height = 100, Width = 800, FontSize = 35,  FontWeight = 3, Z_Index = 1, IsItalic = true, isUnderLine = true, isBold = true, Opacity = 1, X = 50, Y = 750, Color = "Blue", BackgroundColor = "Yellow",FontFamily="Calibri",
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
				
				canvas = await LayerTexts(canvas, template.TextElements);
			}

			await SaveImage(canvas, "newImage.png");
		}

		private Task<GcBitmap> LoadImage(Image image)
		{
			

			//Load image
			return Task.FromResult(ResizeImage(image, template.Width, template.Height));
		}


		private Task<GcBitmap> LayerTexts(GcBitmap bmp, List<TextElement> textElements)
		{
			var g = bmp.CreateGraphics();
			foreach (var item in textElements)
			{
				var jsonStringProduct = JsonConvert.SerializeObject(product);
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
				GcBitmap addedimage = ResizeImage(image, item.Width, item.Height);
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

		private GcBitmap ResizeImage(Image image, int width, int height)
		{
			
			GcBitmap originalImage = new GcBitmap(image);


			//Reduce image 
			if (originalImage.PixelHeight > height || originalImage.PixelWidth > width)
			{
				int rwidth = originalImage.PixelWidth - (originalImage.PixelWidth - width);
				int rheight = originalImage.PixelHeight - (originalImage.PixelHeight - height);
				originalImage = originalImage.Resize(rwidth, rheight,
										  InterpolationMode.Linear); 
			}

			if (originalImage.PixelHeight < height || originalImage.PixelWidth < width)
			{
				//Enlarge image
				int ewidth = originalImage.PixelWidth + (width - originalImage.PixelWidth);
				int eheight = originalImage.PixelHeight + (width - originalImage.PixelWidth);
				originalImage = originalImage.Resize(ewidth, eheight,
										  InterpolationMode.Linear); 
			}

			return originalImage;
		}
	}
}
