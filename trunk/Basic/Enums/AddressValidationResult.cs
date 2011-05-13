namespace Activizr.Basic
{
	public enum AddressValidationResult
	{
		/// <summary>
		/// Unknown or undefined result.
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// Confirmed valid by remote server.
		/// </summary>
		Valid = 1,
		/// <summary>
		/// Confirmed invalid by remote server.
		/// </summary>
		AccountInvalid = 2,
		/// <summary>
		/// Remote server does not respond or exist.
		/// </summary>
		ServerInvalid = 3,
		/// <summary>
		/// Email address syntax is invalid.
		/// </summary>
		BadSyntax = 4
	}
}