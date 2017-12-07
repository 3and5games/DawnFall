using System.Collections.Generic;

namespace Dialoges
{
    [System.Serializable]
    public class ParamChanges
    {
        public Param aimParam;
        public List<Param> Parameters = new List<Param>();
        public string changeString = "";

        public ParamChanges(Param aimParam)
        {
            this.aimParam = aimParam;
        }

        public void RemoveParam(Param p)
        {
            Parameters.Remove(p);
        }

        public void AddParam(Param p)
        {
            Parameters.Add(p);
        }

        public void setParam(Param p, int index)
        {
            Parameters[index] = p;
        }
    }
}