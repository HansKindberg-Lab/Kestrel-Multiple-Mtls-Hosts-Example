using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Application.Pages
{
	public class IndexModel(IOptions<HostFilteringOptions> hostFilteringOptions) : PageModel
	{
		#region Fields

		private readonly IOptions<HostFilteringOptions> _hostFilteringOptions = hostFilteringOptions ?? throw new ArgumentNullException(nameof(hostFilteringOptions));

		#endregion

		#region Properties

		public IList<SelectListItem> Hosts
		{
			get
			{
				if(field == null)
				{
					field = new List<SelectListItem>();

					foreach(var host in this._hostFilteringOptions.Value.AllowedHosts)
					{
						field.Add(new SelectListItem($"https://{host}", $"https://{host}", string.Equals(host, this.Request.Host.Value, StringComparison.OrdinalIgnoreCase)));
					}
				}

				return field;
			}
		}

		#endregion
	}
}