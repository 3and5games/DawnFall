using System.Collections.Generic;

namespace Dialoges
{
    [System.Serializable]
    public class Condition
    {
        public List<Param> Parameters = new List<Param>();
        public string conditionString = "";

        public void RemoveParam(Param p)
        {
            Parameters.Remove(p);
        }
        public void AddParam(Param p)
        {
            Parameters.Add(p);
        }
        public void setParam(int j, Param param)
        {
            Parameters[j] = param;
        }
    }
}



