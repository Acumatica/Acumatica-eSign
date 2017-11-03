using System;
using PX.Data;
using PX.Objects.PO;

namespace AcumaticaESign
{
    public class POOrderESExt : PXCacheExtension<POOrder>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(Search<PORemitContact.contactID>))]
        public virtual Int32? RemitContactID { get; set; }
    }
}
