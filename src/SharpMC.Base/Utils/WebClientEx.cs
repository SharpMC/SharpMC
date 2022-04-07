using System;
using System.Net;

namespace SharpMC.Core.Utils
{
	public class WebClientEx : WebClient
	{
		private WebResponse _mResp;

		public HttpStatusCode StatusCode
		{
			get
			{
				if (_mResp != null && _mResp is HttpWebResponse)
					return (_mResp as HttpWebResponse).StatusCode;
				return HttpStatusCode.OK;
			}
		}

		protected override WebResponse GetWebResponse(WebRequest req, IAsyncResult ar)
		{
			return _mResp = base.GetWebResponse(req, ar);
		}
	}
}