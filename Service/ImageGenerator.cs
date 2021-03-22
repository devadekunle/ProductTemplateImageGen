using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Text;
using ProductTemplateImageGen.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
			};

			template = new Template
			{
				ImageElements = new List<ImageElement>() {
					new ImageElement { Height = 150, Width = 150, ImageUrl = "https://thumbs.dreamstime.com/b/discount-stamp-vector-clip-art-33305813.jpg" } 
				},
				

			};

		

		}
		public async Task GenerateImage()
		{
			var text = @"{name} is simply dummy text of the printing and typesetting industry.{price} has been the industry's standard dummy text ever since the 1500s,when an unknown printer took a of type and scrambled it to make a type specimen book";

			var image = await LoadImage("product-headphone.jpg");
			var mergeImage = await AddImage(image, template);
			var editedImage = await ModifyImage(image, text);
			await SaveImage(editedImage, $"{nameof(editedImage)}-headphone.jpeg");

			//can you run it? What is it producing now?

			//https://stackoverflow.com/questions/2070365/how-to-generate-an-image-from-text-on-fly-at-runtime
		}

		private Task<GcBitmap> LoadImage(string fileName)
		{
			var bmp = new GcBitmap();
			var fs = new FileStream(Path.Combine(fileName), FileMode.Open, FileAccess.ReadWrite);
			
			//Load image
			bmp.Load(fs);
			return Task.FromResult(bmp);
		}


		private Task<GcBitmap> ModifyImage(GcBitmap bmp, string text)
		{
			var g = bmp.CreateGraphics();

			TextLayout tl = g.CreateTextLayout();
			tl.DefaultFormat.FontSize = 70;
			tl.MaxWidth = 1000; // hardcoded
			tl.WrapMode = WrapMode.WordWrap;
			tl.Append(text);
			tl.TextAlignment = TextAlignment.Justified;
			g.DrawTextLayout(tl, new PointF(500, 1000));

			//Save the image rendering different shapes
			return Task.FromResult(bmp);
		}

		private  Task<GcBitmap> AddImage(GcBitmap bmp, Template template)
		{
			
			foreach (var item in template.ImageElements)
			{
				var image = DownloadImageFromUrl(item);

				GcBitmap addedimage = new GcBitmap(image);
				//Combine the two images using various compositing and blending modes
				bmp.CompositeAndBlend(addedimage, 0, 0, CompositeMode.SourceOver, BlendMode.Normal);

			}
			return Task.FromResult(bmp);
		}

		private static Image DownloadImageFromUrl(ImageElement item)
		{
			webClient = (HttpWebRequest)HttpWebRequest.Create(item.ImageUrl);
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
