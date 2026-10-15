using System.Text.RegularExpressions;

namespace Ans.Net10.Web
{

	public enum NodemapTypesEnum
	{
		Node,
		Group,
		InternalPath,
		ExternalUrl
	}



	public sealed partial class NodemapItem
	{

		[GeneratedRegex("^([0-9a-z_-]+)$", RegexOptions.Compiled)]
		private static partial Regex _normalTargetRegex();

		private readonly List<NodemapItem> _childNodes = [];
		private readonly List<PagemapItem> _topPages = [];


		/* ctor */


		public NodemapItem(
			string? target,
			string face)
		{
			Target = target;
			Face = face ?? throw new ArgumentNullException(nameof(face));
			Name = Target;
			ChildNodes = _childNodes.AsReadOnly();
			TopPages = _topPages.AsReadOnly();
			if (string.IsNullOrEmpty(target))
			{
				Type = NodemapTypesEnum.Group;
				Name = null;
			}
			else if (target.StartsWith('/'))
			{
				Type = NodemapTypesEnum.InternalPath;
				Name = null;
			}
			else if (_normalTargetRegex().IsMatch(target))
			{
				Type = NodemapTypesEnum.Node;
				Name = target;
			}
			else
			{
				Type = NodemapTypesEnum.ExternalUrl;
				Name = null;
			}
		}


		/* readonly properties */


		public string? Target { get; }


		public string Face { get; }


		public string? Name { get; }


		public NodemapTypesEnum Type { get; }


		public NodemapItem? ParentNode { get; internal set; }


		public IReadOnlyCollection<NodemapItem> ChildNodes { get; }


		public IReadOnlyCollection<PagemapItem> TopPages { get; }


		public IEnumerable<NodemapItem> ParentNodes
		{
			get
			{
				var current1 = ParentNode;
				while (current1 != null)
				{
					yield return current1;
					current1 = current1.ParentNode;
				}
			}
		}


		/* internals */


		internal void _AddChildNode(
			NodemapItem child)
		{
			child.ParentNode = this;
			_childNodes.Add(child);
		}


		internal void _AddTopPage(
			PagemapItem page)
		{
			_topPages.Add(page);
		}

	}

}