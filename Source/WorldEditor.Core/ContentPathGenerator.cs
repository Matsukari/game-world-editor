

namespace WorldEditor
{
    /// <summary>
    /// class that contains the names of all of the files processed by the Pipeline Tool
    /// </summary>
    /// <remarks>
    /// WorldEditor includes a T4 template that will auto-generate the content of this file.
    /// See: https://github.com/prime31/WorldEditor/blob/master/FAQs/ContentManagement.md#auto-generating-content-paths"
    /// </remarks>
    class Assets
    {
		public static class Sheets
		{
			public const string Big_forest = @"Content/Sheets/big_forest";
			public const string Body_parts = @"Content/Sheets/body_parts";
			public const string Deerbreathe = @"Content/Sheets/deer-breathe";
		}

		public const string Content = @"Content/Content.mgcb";
		public const string NuGet = @"Content/NuGet.config";
		public static class Fonts
		{
			public const string FontAwesome6BrandsRegular400 = @"Fonts/Font Awesome 6 Brands-Regular-400.otf";
			public const string FontAwesome6FreeRegular400 = @"Fonts/Font Awesome 6 Free-Regular-400.otf";
			public const string FontAwesome6FreeSolid900 = @"Fonts/Font Awesome 6 Free-Solid-900.otf";
			public const string RobotoCondensedRegular = @"Fonts/RobotoCondensed-Regular.ttf";
		}

		public static class Images
		{
			public const string Icon = @"Images/icon";
		}


    }
}

