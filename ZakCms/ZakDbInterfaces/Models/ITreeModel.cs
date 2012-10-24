using System;
using System.Collections.Generic;

namespace ZakDb.Models
{
	public interface ITreeModel : IModel
	{
		List<ITreeModel> Children { get; set; }
		Int64 ParentId { get; set; }
		Int64 LeftNs { get; set; }
		Int64 RightNs { get; set; }
		Int32 Ordering { get; set; }
	}
}