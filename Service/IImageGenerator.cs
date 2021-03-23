using ProductTemplateImageGen.Model;
using System.Threading.Tasks;

namespace ProductTemplateImageGen.Service
{
	public interface IImageGenerator
    {
        Task GenerateProductTemplate();
    }
}