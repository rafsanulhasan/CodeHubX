using Android.App;
using Android.Content;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	public class FileStorage : IFileStorage
	{
		private Context _context = Application.Context;

		public async Task<byte[]> ReadAsBytes(string filename)
			=> Encoding.UTF8.GetBytes(await ReadAsString(filename));

		public async Task<string> ReadAsString(string filename)
		{
			using (var asset = _context.Assets.Open(filename))
			using (var streamReader = new StreamReader(asset))
			{
				return await streamReader.ReadToEndAsync();
			}
		}
	}
}