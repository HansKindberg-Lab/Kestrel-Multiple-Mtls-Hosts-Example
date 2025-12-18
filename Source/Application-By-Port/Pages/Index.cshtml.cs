using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Application.Pages
{
	public class IndexModel : PageModel
	{
		#region Fields

		private static readonly IDictionary<string, IList<int>> _hosts = new Dictionary<string, IList<int>>
		{
			{ "Default", [OperatingSystem.IsWindows() ? 5100 : 5000] },
			{ "Mtls", [OperatingSystem.IsWindows() ? 5101 : 5001] },
			{ "Client-1", OperatingSystem.IsWindows() ? [5102, 5103, 5104, 5105, 5106] : [5002, 5003, 5004, 5005, 5006] },
			{ "Client-2", OperatingSystem.IsWindows() ? [5107, 5108, 5109, 5110, 5111] : [5007, 5008, 5009, 5010, 5011] },
			{ "Client-3", OperatingSystem.IsWindows() ? [5112, 5113, 5114, 5115, 5116] : [5012, 5013, 5014, 5015, 5016] },
			{ "Client-4", OperatingSystem.IsWindows() ? [5117, 5118, 5119, 5120, 5121] : [5017, 5018, 5019, 5020, 5021] }
		};

		#endregion

		#region Properties

		public IList<SelectListItem> Hosts
		{
			get
			{
				if(field == null)
				{
					field = new List<SelectListItem>();

					foreach(var entry in _hosts)
					{
						foreach(var port in entry.Value)
						{
							field.Add(new SelectListItem($"{entry.Key}: https://localhost:{port}", $"https://localhost:{port}", port == this.Request.Host.Port));
						}
					}
				}

				return field;
			}
		}

		#endregion
	}
}