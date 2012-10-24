using System;
using ZakDb.Models;

namespace ZakCms.Models.Entitites
{
	public class LanguageModel : ILovModel
	{
		public LanguageModel(Int64 id = -1)
		{
			Id = id;
			Code = "en-EN";
			Description = string.Empty;
		}

		public Int64 Id { get; set; }
		public String Code { get; set; }
		public String Description { get; set; }
	}
}