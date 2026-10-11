using Ans.Net10.Web.Services;

namespace Ans.Net10.Web
{

	public class MapsData(
		CurrentContext current,
		IMapNodesProvider mapNodesProvider,
		IMapPagesProvider mapPagesProvider)
	{

		private readonly CurrentContext _current = current;
		private readonly IMapNodesProvider _mapNodesProvider = mapNodesProvider;
		private readonly IMapPagesProvider _mapPagesProvider = mapPagesProvider;


		/* functions */


		public MapNodes GetMapNodes()
		{
			var map1 = _mapNodesProvider.GetMapNodes(
				_current.Host.VirtualPath);
			return map1;
		}


		public MapPages GetMapPages(
			string node)
		{	
			var map1 = _mapPagesProvider.GetMapPages(
				node, _current.Host.VirtualPath);
			return map1;
		}


		/* methods */


		public void ResetNodes()
		{
			foreach (var node1 in GetMapNodes().AllItems)
				ResetPages(node1.Target);
			_mapNodesProvider.Refresh();
		}


		public void ResetPages(
			string node)
		{
			_mapPagesProvider.Refresh(node ?? "_main");
		}

	}

}
