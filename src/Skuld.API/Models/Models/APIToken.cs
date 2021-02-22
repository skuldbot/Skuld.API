namespace Skuld.API.Models
{
	public class APIToken
	{
		public ulong Id { get; set; }
		public ulong ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string Token { get; set; }
		public ulong OwnerId { get; set; }
		public string Description { get; set; }
		public ulong TotalAPICalls { get; set; }
		public bool IsValid { get; set; }
	}
}
