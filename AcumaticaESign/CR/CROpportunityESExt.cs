using System;
using PX.Data;
using PX.Objects.CR;

namespace AcumaticaESign
{
    public class CROpportunityESExt : PXCacheExtension<CROpportunity>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(CRContact.contactID), ValidateValue = false)]
        public virtual Int32? OpportunityContactID { get; set; }
    }
}