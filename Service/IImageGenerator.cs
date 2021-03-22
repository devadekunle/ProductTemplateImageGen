using System.Threading.Tasks;

namespace ProductTemplateImageGen.Service
{
	public interface IImageGenerator
	{
		Task GenerateImage();
	}
}