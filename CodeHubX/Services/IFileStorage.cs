using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public interface IFileStorage
	{
		Task<string> ReadAsString(string filename);

		Task<byte[]> ReadAsBytes(string filename);
	}
}
