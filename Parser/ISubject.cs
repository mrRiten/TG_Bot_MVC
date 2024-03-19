namespace Parser
{
    public interface ISubject
    {
        public void Notify();
        public void AddObserver(IObserver observer);
        public void RemoveObserver(IObserver observer);
    }
}
