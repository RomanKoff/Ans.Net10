using System.Text.RegularExpressions;

namespace Ans.Net10.Web
{

	public enum PagemapTypesEnum
	{
		Page,
		Catalog,
		InternalPath,
		ExternalUrl
	}



	public sealed partial class PagemapItem
	{

		[GeneratedRegex("^([0-9a-z_-]+)$", RegexOptions.Compiled)]
		private static partial Regex _normalTargetRegex();

		private readonly List<PagemapItem> _childPages = [];


		/* ctor */


		public PagemapItem(
			NodemapItem node,
			string target,
			string face,
			bool hasNoIndex)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
			Target = target ?? throw new ArgumentNullException(nameof(target));
			Face = face ?? throw new ArgumentNullException(nameof(face));
			HasNoIndex = hasNoIndex;
			if (target.StartsWith('/'))
			{
				Type = PagemapTypesEnum.InternalPath;
				Name = null;
			}
			else if (_normalTargetRegex().IsMatch(target))
			{
				Type = PagemapTypesEnum.Page;
				Name = target;
			}
			else
			{
				Type = PagemapTypesEnum.ExternalUrl;
				Name = null;
			}
			ChildPages = _childPages.AsReadOnly();
		}


		/* readonly properties */


		public NodemapItem Node { get; }


		public string Target { get; }


		public string Face { get; }


		public bool HasNoIndex { get; }


		public string? Name { get; }


		public PagemapTypesEnum Type { get; private set; }


		public PagemapItem? ParentPage { get; private set; }


		public IReadOnlyCollection<PagemapItem> ChildPages { get; }


		public string? Path
			=> field ??= _buildPath();


		public IEnumerable<PagemapItem> ParentPages
		{
			get
			{
				var current1 = ParentPage;
				while (current1 != null)
				{
					yield return current1;
					current1 = current1.ParentPage;
				}
			}
		}


		/* internals */


		internal void _AddChildPage(
			PagemapItem child)
		{
			if (Type != PagemapTypesEnum.Page)
				return;
			Type = PagemapTypesEnum.Catalog;
			child.ParentPage = this;
			_childPages.Add(child);
		}


		/* privates */


		private string? _buildPath()
		{
			if (string.IsNullOrEmpty(Name))
				return null;
			var parts1 = new List<string>();
			foreach (var ancestor1 in ParentPages)
				if (!string.IsNullOrEmpty(ancestor1.Name))
					parts1.Insert(0, ancestor1.Name);
			parts1.Add(Name);
			return string.Join('/', parts1);
		}

	}

}