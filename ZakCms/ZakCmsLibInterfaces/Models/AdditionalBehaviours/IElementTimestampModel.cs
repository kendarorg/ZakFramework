using System;
using ZakDb.Models;

namespace ZakCms.Models.AdditionalBehaviours
{
	public interface IElementTimestampModel : IModel
	{
		DateTime CreateTime { get; set; }
		DateTime UpdateTime { get; set; }
	}
}