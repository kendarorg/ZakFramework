using System;
using ZakDb.Models;

namespace ZakCms.Models.Joins
{
	public interface IJoinedTagModel : IModel
	{
		Int64 TagId { get; set; }
		string TagCode { get; set; }
		string TagDescription { get; set; }
	}
}