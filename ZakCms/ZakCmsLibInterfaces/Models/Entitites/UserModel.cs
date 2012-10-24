using System;
using ZakDb.Models;

namespace ZakCms.Models.Entitites
{
	public class UserModel : IModel
	{
		public Int64 Id { get; set; }
		public String UserId { get; set; }
		public String UserPassword { get; set; }
	}
}