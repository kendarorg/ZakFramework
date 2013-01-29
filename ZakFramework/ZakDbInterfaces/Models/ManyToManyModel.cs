using System;

namespace ZakDb.Models
{
	public class ManyToManyModel : IModel
	{
		public ManyToManyModel(Int64 id = -1)
		{
			Id = id;
		}

		public Int64 Id { get; set; }
		public Int64 LeftId { get; set; }
		public Int64 RightId { get; set; }
		public IModel Content { get; set; }
	}
}