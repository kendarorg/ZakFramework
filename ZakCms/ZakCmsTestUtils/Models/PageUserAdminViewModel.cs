using System;
using System.Collections.Generic;
using ZakCms.Models.Entitites;

namespace ZakCms.Models
{
	public class PageUserAdminViewModel
	{
		public PageUserAdminViewModel()
		{
			View = "Details";
			Users = new List<UserModel>();
		}

		public List<UserModel> Users { get; private set; }
		public UserModel User { get; set; }

		public Int64 Id { get; set; }
		public string View { get; set; }
	}
}