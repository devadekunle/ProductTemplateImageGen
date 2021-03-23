using Microsoft.AspNetCore.Mvc;
using ProductTemplateImageGen.Service;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Controllers
{
	[ApiController]
	[Route("api/[Controller]")]
	public class ImageGenController : ControllerBase
	{
		private readonly IImageGenerator _imageGenerator;
		public ImageGenController(IImageGenerator imageGenerator)
		{
			_imageGenerator = imageGenerator;
		}
		[HttpGet]
		public async Task<IActionResult> Get()
        {
            await _imageGenerator.GenerateProductTemplate();
			return Ok();
		}
	}
}
