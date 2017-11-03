using PX.Data;
using PX.SM;

namespace AcumaticaESign
{
    /// <summary>
    /// Represent inqury logic fo the <see cref="ESignRecipient"/> recipients
    /// Records of this type can be viewed through the (DS.30.30.00) screen.
    /// </summary>
    public class ESignEnvelopeEnq : PXGraph<ESignEnvelopeEnq>
    {
        [PXFilterable]
        public PXSelectJoin<ESignEnvelopeInfo, 
            InnerJoin<UploadFileWithIDSelector, 
                On<UploadFileWithIDSelector.fileID, Equal<ESignEnvelopeInfo.fileID>>>> Envelopes;
        public PXSelectReadonly<ESignRecipient, 
            Where<ESignRecipient.envelopeInfoID, 
                Equal<Current<ESignEnvelopeInfo.envelopeInfoID>>,
                And<ESignRecipient.status, IsNotNull>>> Recipients;
    }
}
