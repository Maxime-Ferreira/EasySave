using System.Threading;

namespace Core.Model
{
    public interface ISaveWorkModel
    {
        public void SaveData(Barrier barrier);
        public string GetType();

        public string GetName();
    }
}
