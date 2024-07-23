namespace Spade.LifeCycle
{
	public abstract class HaveLifeCycle
	{
		public virtual void Awake()     { }
		public virtual void Start()     { }
		public virtual void Update()    { }
		public virtual void OnDestroy() { }
		public virtual void OnGUI()     { }
	}
}