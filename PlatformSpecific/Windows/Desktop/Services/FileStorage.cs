using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CodeHubX.Services
{
	using static Helpers.ArrayHelper;

	public class FileStorage : IFileStorage
	{
		public Task<byte[]> ReadAsBytes(string filename)
		{
			var data = File.ReadAllBytes(filename);

			if (data != null)
				data = data.CleanByteOrderMark();

			return Task.FromResult(data);
		}

		public async Task<string> ReadAsString(string filename)
		{
			var data = await ReadAsBytes(filename);

			if (data == null)
				return string.Empty;

			return Encoding.UTF8.GetString(data);
		}
	}
}
