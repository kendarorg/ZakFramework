using System;
using ZakDb.Exceptions;

namespace ZakDb.Repositories.Exceptions
{
	public class RepositoryValidationException : ZakException
	{
		public string Field { get; private set; }
		public string Error { get; private set; }

		public RepositoryValidationException(string originatingModule, string operation, string field, string cause,
		                                     Exception ex) :
			                                     base(
			                                     string.Format(
				                                     "Error doing operation '{0}' on field '{1}' within module '{2}': {3}",
				                                     originatingModule, operation, field, cause), ex)
		{
			Field = field;
			Error = cause;
		}

		public RepositoryValidationException(string originatingModule, string operation, string field, string cause) :
			base(
			string.Format("Error doing operation '{0}' on field '{1}' within module '{2}': {3}", originatingModule, operation,
			              field, cause))
		{
			Field = field;
			Error = cause;
		}
	}
}