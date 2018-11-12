using GalaSoft.MvvmLight;
using System;

namespace CodeHubX.Models
{
	public class SettingsItem : ObservableObject
	{
		public string MainText { get; set; }
		public string SubText { get; set; }
		public string GlyphString { get; set; }
		public Type DestPage { get; set; }
	}
}
