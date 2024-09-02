namespace ServerApp.Services
{
	public interface IAWS
	{
		public void store(IModel model);
		public void delete(string key = null);
	}
}
