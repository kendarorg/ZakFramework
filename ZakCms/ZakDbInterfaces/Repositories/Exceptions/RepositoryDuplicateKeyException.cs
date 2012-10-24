using System;
using ZakDb.Exceptions;

namespace ZakDb.Repositories.Exceptions
{
	public class RepositoryDuplicateKeyException : ZakException
	{
		public RepositoryDuplicateKeyException(string originatingModule, string operation, Exception ex) :
			base(
			string.Format("Error doing operation '{0}' within module '{1}': Duplicate key not allowed", operation,
			              originatingModule), ex)
		{
		}

		public RepositoryDuplicateKeyException(string originatingModule, string operation) :
			base(
			string.Format("Error doing operation '{0}' within module '{1}': Duplicate key not allowed", operation,
			              originatingModule))
		{
		}
	}
}