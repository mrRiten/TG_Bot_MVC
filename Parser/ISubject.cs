using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public interface ISubject
    {
        public void Notify();
        public void AddObserver(IObserver observer);
        public void RemoveObserver(IObserver observer);
    }
}
