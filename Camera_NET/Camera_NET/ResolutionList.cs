namespace Camera_NET
{
    using System;
    using System.Collections.Generic;

    public class ResolutionList : List<Resolution>
    {
        public bool AddIfNew(Resolution item)
        {
            if (base.Contains(item))
            {
                return false;
            }
            base.Add(item);
            return true;
        }
    }
}

