using System;
using ZakDb.Models;

namespace ZakCms.Models.Entitites
{
	public class CompanyModel : ILovModel
	{
		public CompanyModel(Int64 id = -1)
		{
			Id = id;
		}

		public Int64 Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
	}
}