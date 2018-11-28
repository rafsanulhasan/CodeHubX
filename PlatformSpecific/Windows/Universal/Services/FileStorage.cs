using CodeHubX.Helpers;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CodeHubX.Services
{
	public class FileStorage : IFileStorage
	{
		public async Task<string> ReadAsString(string filename)
		{
			var bytes = await ReadAsBytes(filename);
			return Encoding.UTF8.GetString(bytes.CleanByteOrderMark());
			//return Encoding.UTF8.GetString(bytes);
		}

		public async Task<byte[]> ReadAsBytes(string filename)
		{
			//var folderStructure = "Assets/";
			var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{filename}"));

			var buffer = await FileIO.ReadBufferAsync(file);
			using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
			{
				var bytes = new byte[buffer.Length];
				dataReader.ReadBytes(bytes);
				return bytes;
			}
		}
	}
}