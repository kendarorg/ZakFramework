using System;
using ZakDb.Models;

namespace ZakCms.Models.Entitites
{
	public class TagModel : ILovModel
	{
		public Int64 Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
	}
}