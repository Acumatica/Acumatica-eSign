using System;
using PX.Data;
using PX.Objects.AR;

namespace AcumaticaESign
{
    public class ARInvoiceESExt : PXCacheExtension<ARInvoice>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(Search<ARContact.contactID>), ValidateValue = false)]
        public virtual Int32? BillContactID { get; set; }
    }
}