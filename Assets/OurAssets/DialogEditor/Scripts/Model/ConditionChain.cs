
namespace Dialoges
{
    [System.Serializable]
    public class ConditionChain
    {
        public Condition c;
        public int stateGuid;
        public int chainGuid;

        public ConditionChain(Condition key, int chainGuid, int stateGuid)
        {
            this.c = key;
            this.chainGuid = chainGuid;
            this.stateGuid = stateGuid;
        }
    }
}
