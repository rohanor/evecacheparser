using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace EveCacheParser.STypes
{
    public sealed class STypeCollection : Collection<SType>
    {
        #region Constructors

        internal STypeCollection()
            : base(new List<SType>())
        {
        }

        internal STypeCollection(IEnumerable<SType> source)
            : base(new List<SType>(source))
        {
        }

        #endregion


        #region Properties

        public int Length
        {
            get { return Items.Count(); }
        }

        #endregion
    }
}
