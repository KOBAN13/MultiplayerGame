using Db.Interface;
using UnityEngine;

namespace Db.Server
{
    [CreateAssetMenu(fileName = "PredictionParameters", menuName = "Db/PredictionParameters")]
    public class PredictionParameters : ScriptableObject, IPredictionParameters
    {
        [field: SerializeField] public float SmallError { get; private set; }
        [field: SerializeField] public float MediumError { get; private set; }
        [field: SerializeField] public float LargeError { get; private set; }
        [field: SerializeField] public float VisualHalfLife { get; private set; }
        [field: SerializeField] public float MediumMoveGain { get; private set; }
        
        [field: SerializeField] public int MaxBufferSize { get; private set; }
        [field: SerializeField] public int CountGenerateStateSendToServer { get; private set; }
        [field: SerializeField] public int CountGenerateStateLocalSimulation { get; private set; }
    }
}