namespace UI.Base
{
    public interface IPayloadReceiver<in TPayload>
    {
        void ApplyPayload(TPayload payload);
    }
}
