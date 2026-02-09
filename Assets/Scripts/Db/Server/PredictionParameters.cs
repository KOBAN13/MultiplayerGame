using Db.Interface;
using UnityEngine;

namespace Db.Server
{
    [CreateAssetMenu(fileName = "PredictionParameters", menuName = "Db/PredictionParameters")]
    public class PredictionParameters : ScriptableObject, IPredictionParameters
    {
        [field: SerializeField] public int MaxBufferSize { get; private set; }
    }
}