namespace ESC.Patron
{
	public abstract class EcsComponent
	{
		private uint _entityOwnerID = 0;

		public uint EntityOwnerID { get => _entityOwnerID; set => _entityOwnerID = value; }

		protected EcsComponent() { }

		public virtual void Dispose() { }
	}
}
