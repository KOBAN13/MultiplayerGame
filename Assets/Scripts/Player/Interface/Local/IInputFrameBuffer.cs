using Input;

namespace Player.Interface.Local
{
    public interface IInputFrameBuffer
    {
        int Count { get; }
        void Enqueue(in InputFrame predictionStateFrame);
        bool TryDequeue(out InputFrame predictionStateFrame);
        void Clear();
    }
}
