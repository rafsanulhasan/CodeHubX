using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public class FileStorage : IFileStorage
	{
		public async Task<byte[]> ReadAsBytes(string filename)
			=> Encoding.UTF8.GetBytes(await ReadAsString(filename));

		public async Task<string> ReadAsString(string filename)
		{
			using (var asset = File.Open(filename, FileMode.Open))
			using (var streamReader = new StreamReader(asset))
			{
				return await streamReader.ReadToEndAsync();
			}
		}
	}
}