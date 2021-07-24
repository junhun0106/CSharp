
namespace ChatService
{
    public enum EPropagate
    {
        Add,
        Remove,
    }

    public interface IPropagate
    {
        EPropagate Propagate { get; }
    }

    public class AddPropagate : IPropagate
    {
        public EPropagate Propagate { get; } = EPropagate.Add;
    }
}
