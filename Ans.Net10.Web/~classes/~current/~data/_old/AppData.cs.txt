using Ans.Net10.Common;
using Microsoft.AspNetCore.Html;

namespace Ans.Net10.Web
{

	public class AppData(
		CurrentContext current)
	{

		private readonly CurrentContext _current = current;

		private static Dictionary<string, string> _endpoints;
		private static Dictionary<string, DictString> _dicts;
		private static Dictionary<string, DictHtml> _dictsHtml;
		private static Dictionary<string, string[]> _addresses;
		private static Dictionary<string, string> _addressesShort;
		private static Dictionary<string, HtmlString[]> _addressesHtml;
		private static Dictionary<string, MediaLinkDef> _medias;
		private static Dictionary<string, string> _values;
		private static Dictionary<string, HtmlString> _valuesHtml;


		/* functions */


		// Endpoints


		/// <summary>
		/// Возвращает точку подключения соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Endpoints
		/// </summary>
		/// <param name="query">параметры запроса</param>
		public string GetEndpoint(
			string key,
			string query = null)
		{
			_endpoints ??= getEndpoints();
			var s1 = _endpoints.GetValueOrDefault(key, null)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Endpoints/{key}] is required!");
			return string.IsNullOrEmpty(query)
				? s1 : $"{s1}?{query}";
		}


		// Dicts


		/// <summary>
		/// Возвращает DictString соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Dicts
		/// </summary>
		public DictString GetDict(
			string key,
			DictString defaultValue = null)
		{
			if (_dicts == null)
			{
				var d1 = getDicts();
				_dicts = new Dictionary<string, DictString>();
				foreach (var item1 in d1)
					_dicts.Add(item1.Key, new DictString(item1.Value));
			}
			return _dicts.GetValueOrDefault(key, defaultValue)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Dicts/{key}] is required!");
		}


		/// <summary>
		/// Возвращает DictHtml соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Dicts
		/// </summary>
		public DictHtml GetDictHtml(
			string key,
			DictHtml defaultValue = null)
		{
			if (_dictsHtml == null)
			{
				var d1 = getDicts();
				_dictsHtml = new Dictionary<string, DictHtml>();
				foreach (var item1 in d1)
					_dictsHtml.Add(item1.Key, new DictHtml(item1.Value));
			}
			return _dictsHtml.GetValueOrDefault(key, defaultValue)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Dicts/{key}] is required!");
		}


		/// <summary>
		/// Возвращает string соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Dicts/{name}
		/// </summary>
		public string GetDictValue(
			string name,
			string key)
		{
			return GetDict(name).GetValueOrDefault(key)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Dicts/{name}/{key}] is required!");
		}


		/// <summary>
		/// Возвращает HtmlString соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Dicts/{name}
		/// </summary>
		public HtmlString GetDictValueHtml(
			string name,
			string key)
		{
			return GetDictHtml(name).GetValueOrDefault(key)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Dicts/{name}/{key}] is required!");
		}


		// Addresses


		/// <summary>
		/// Возвращает словарь адресов.
		/// Адрес состоит из string[]
		/// в формате [ "Краткая форма", "Полная форма", "Почтовый адрес", "Описание" ]
		/// </summary>
		public Dictionary<string, string[]> GetAddresses()
		{
			if (_addresses == null)
			{
				var d1 = getAddresses();
				_addresses = new Dictionary<string, string[]>();
				foreach (var item1 in d1)
					_addresses.Add(
						item1.Key,
						item1.Value.SplitFix("|", 4));
			}
			return _addresses;
		}


		/// <summary>
		/// Возвращает словарь адресов в краткой форме
		/// </summary>
		public Dictionary<string, string> GetAddressesShort()
		{
			if (_addressesShort == null)
			{
				_addressesShort = new Dictionary<string, string>();
				foreach (var item1 in GetAddresses())
					_addressesShort.Add(
						item1.Key,
						item1.Value[0]);
			}
			return _addressesShort;
		}


		/// <summary>
		/// Возвращает словарь адресов.
		/// Адрес состоит из HtmlString[]
		/// в формате [ "Краткая форма", "Полная форма", "Почтовый адрес", "Описание" ]
		/// </summary>
		public Dictionary<string, HtmlString[]> GetAddressesHtml()
		{
			if (_addressesHtml == null)
			{
				_addressesHtml = new Dictionary<string, HtmlString[]>();
				foreach (var item1 in GetAddresses())
					_addressesHtml.Add(
						item1.Key,
						[.. item1.Value.Select(x => x.ToHtml(true))]);
			}
			return _addressesHtml;
		}


		/// <summary>
		/// Возвращает адрес в формате
		/// string [ "Краткая форма", "Полная форма", "Почтовый адрес", "Описание" ]
		/// </summary>
		public string[] GetAddressSet(
			string key)
		{
			var a1 = GetAddresses();
			return a1.GetValueOrDefault(key)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Addresses/{key}] is required!");
		}


		/// <summary>
		/// Возвращает адрес в формате
		/// HtmlString [ "Краткая форма", "Полная форма", "Почтовый адрес", "Описание" ]
		/// </summary>
		public HtmlString[] GetAddressSetHtml(
			string key)
		{
			var a1 = GetAddressesHtml();
			return a1.GetValueOrDefault(key)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Addresses/{key}] is required!");
		}


		// Medias


		/// <summary>
		/// Возвращает MediaLink соответствующую {url}
		/// соответсвующую шаблону из словаря appsettings.json/Ans.Net10.Web/Medias
		/// </summary>
		public MediaLink GetMedia(
			string url)
		{
			if (_medias == null)
			{
				var d1 = getMedias();
				_medias = new Dictionary<string, MediaLinkDef>();
				foreach (var item1 in d1)
					_medias.Add(item1.Key, new MediaLinkDef(item1.Value));
			}
			foreach (var item1 in _medias)
			{
				var p1 = url.IndexOf(item1.Key);
				if (p1 > -1)
					return new MediaLink
					{
						UrlPart = url[(p1 + item1.Key.Length)..],
						Type = item1.Value.Type,
						Inner = item1.Value.Inner,
					};
			}
			return null;
		}


		// Values


		/// <summary>
		/// Возвращает string соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Values
		/// </summary>
		public string GetValue(
			string key,
			string defaultValue = null)
		{
			_values ??= getValues();
			return _values.GetValueOrDefault(key, defaultValue)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Values/{key}] is required!");
		}


		/// <summary>
		/// Возвращает HtmlString соответствующую {key}
		/// из словаря appsettings.json/Ans.Net10.Web/Values
		/// </summary>
		public HtmlString GetValueHtml(
			string key,
			HtmlString defaultValue = null)
		{
			if (_valuesHtml == null)
			{
				_valuesHtml = new Dictionary<string, HtmlString>();
				foreach (var item1 in getValues())
					_valuesHtml.Add(item1.Key, item1.Value.ToHtml(true));
			}
			return _valuesHtml.GetValueOrDefault(key, defaultValue)
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Values/{key}] is required!");
		}


		/* privates */


		private Dictionary<string, string> getEndpoints()
		{
			Console.WriteLine("[CurrentContext] Init App Endpoints.");
			return _current.Configuration.GetLibWebOptions().Endpoints
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Endpoints] is required!");
		}


		private Dictionary<string, string> getDicts()
		{
			Console.WriteLine("[CurrentContext] Init App Dicts.");
			return _current.Configuration.GetLibWebOptions().Dicts
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Dicts] is required!");
		}


		private Dictionary<string, string> getAddresses()
		{
			Console.WriteLine("[CurrentContext] Init App Addresses.");
			return _current.Configuration.GetLibWebOptions().Addresses
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Addresses] is required!");
		}


		private Dictionary<string, string> getMedias()
		{
			Console.WriteLine("[CurrentContext] Init App Medias.");
			return _current.Configuration.GetLibWebOptions().Medias
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Medias] is required!");
		}


		private Dictionary<string, string> getValues()
		{
			Console.WriteLine("[CurrentContext] Init App Values.");
			return _current.Configuration.GetLibWebOptions().Values
				?? throw new Exception(
					$"[appsettings.json/Ans.Net10.Web/Values] is required!");
		}

	}

}
