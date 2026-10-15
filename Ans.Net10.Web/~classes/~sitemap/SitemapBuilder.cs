using System.Collections.Frozen;

namespace Ans.Net10.Web
{

	public sealed class SitemapBuilder
	{

		private readonly List<NodemapItem> _topNodes = [];

		private FrozenDictionary<string, NodemapItem> _nodesIndex = FrozenDictionary<string, NodemapItem>.Empty;
		private FrozenDictionary<string, PagemapItem> _pagesIndex = FrozenDictionary<string, PagemapItem>.Empty;


		/* ctor */


		public SitemapBuilder()
		{
			TopNodes = _topNodes.AsReadOnly();
		}


		/* readonly properties */


		public IReadOnlyCollection<NodemapItem> TopNodes { get; }


		/* functions */


		public NodemapItem? GetNode(
			string? nodeName)
		{
			if (string.IsNullOrEmpty(nodeName))
				return null;
			_nodesIndex.TryGetValue(nodeName, out var node1);
			return node1;
		}


		public PagemapItem? GetPage(
			string? combinedPath)
		{
			if (string.IsNullOrEmpty(combinedPath))
				return null;
			_pagesIndex.TryGetValue(combinedPath, out var page1);
			return page1;
		}


		/* methods */


		public void AddTopNode(
			NodemapItem node)
		{
			if (node is not null)
				_topNodes.Add(node);
		}


		public void InsertTopNode(
			int index,
			NodemapItem node)
		{
			if (node is not null)
				_topNodes.Insert(index, node);
		}


		public void BuildIndexes()
		{
			var nodesDict1 = new Dictionary<string, NodemapItem>(StringComparer.OrdinalIgnoreCase);
			var pagesDict1 = new Dictionary<string, PagemapItem>(StringComparer.OrdinalIgnoreCase);
			foreach (var node1 in _topNodes)
				_indexNodeRecursive(node1, nodesDict1, pagesDict1);
			_nodesIndex = nodesDict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
			_pagesIndex = pagesDict1.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
		}


		/* privates */


		private static void _indexNodeRecursive(
			NodemapItem node,
			Dictionary<string, NodemapItem> nodesDict,
			Dictionary<string, PagemapItem> pagesDict)
		{
			if (!string.IsNullOrEmpty(node.Name))
				nodesDict[node.Name] = node;
			foreach (var page1 in node.TopPages)
				_indexPageRecursive(page1, node.Name, pagesDict);
			foreach (var childNode1 in node.ChildNodes)
				_indexNodeRecursive(childNode1, nodesDict, pagesDict);
		}


		private static void _indexPageRecursive(
			PagemapItem page,
			string? nodeName,
			Dictionary<string, PagemapItem> pagesDict)
		{
			if (!string.IsNullOrEmpty(nodeName) && !string.IsNullOrEmpty(page.Path))
			{
				string fullKey1 = $"{nodeName}/{page.Path}";
				pagesDict[fullKey1] = page;
			}
			foreach (var childPage1 in page.ChildPages)
				_indexPageRecursive(childPage1, nodeName, pagesDict);
		}

	}

}
