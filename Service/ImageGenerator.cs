using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using ProductTemplateImageGen.Model;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
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
						Height = 500, Width = 1000, FontSize = 25, FontWeight = 3, Z_Index = 1, IsItalic = false, isBold = true, Opacity = 1, X = 100, Y = 1200,
							  Text = @"{name} is simply dummy text of the printing and typesetting industry.{price} has been the industry's standard dummy text ever since the 1500s,when an unknown printer took a of type and scrambled it to make a type specimen book"
						  }, 
					new TextElement
						  {
								Height = 500, Width = 1000, FontSize = 30, FontWeight = 3, Z_Index = 1, IsItalic = false, isBold = true, Opacity = 1, X = 100, Y = 1400, 
							  Text = "{price} has been the industry's standard dummy text ever since the 1500s,when an unknown printer took a of type and scrambled it to make a type specimen book"
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
				var processedTextElements = GenerateInterpolatedText(template.TextElements, product);
				canvas = await ModifyImage(canvas, processedTextElements);
			}

			await SaveImage(canvas, "newImage.png");
		}

		private List<TextElement> GenerateInterpolatedText(List<TextElement> textElements, Product product)
		{
			return textElements;
		}

		private Task<GcBitmap> LoadImage(Image image)
		{
			var bmp = new GcBitmap(image);

			//Load image
			return Task.FromResult(bmp);
		}


		private Task<GcBitmap> ModifyImage(GcBitmap bmp, List<TextElement> textElements)
		{
			var g = bmp.CreateGraphics();
			foreach (var item in textElements)
			{


				TextLayout tl = g.CreateTextLayout();
				tl.DefaultFormat.FontSize = item.FontSize;
				tl.DefaultFormat.FontBold = item.isBold;
				tl.DefaultFormat.FontItalic = item.IsItalic;
				tl.MaxWidth = item.Width; // hardcoded
				tl.MaxHeight = item.Height;
				tl.WrapMode = WrapMode.WordWrap;
				tl.Append(item.Text);
				tl.TextAlignment = TextAlignment.Justified;
				g.DrawTextLayout(tl, new PointF(item.X, item.Y));
			}

			//Save the image rendering different shapes
			return Task.FromResult(bmp);
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
