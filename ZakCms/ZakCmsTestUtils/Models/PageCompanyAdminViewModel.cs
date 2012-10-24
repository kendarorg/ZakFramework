using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCms.Models
{
	public class PageCompanyAdminViewModel
	{
		public PageCompanyAdminViewModel()
		{
			View = "Details";
			Companies = new List<CompanyModel>();
		}

		public List<CompanyModel> Companies { get; private set; }
		public CompanyModel Company { get; set; }

		public Int64 Id { get; set; }
		public string View { get; set; }
	}
}