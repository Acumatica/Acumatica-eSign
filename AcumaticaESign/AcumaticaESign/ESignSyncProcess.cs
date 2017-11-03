using System.Collections;
using AcumaticaESign.SM;
using PX.Data;
using PX.Objects.CR;
using PX.SM;
namespace AcumaticaESign
{
    /// <summary>
    /// Represent synchronization logic for <see cref="ESignEnvelopeInfo"/> envelopes.
    /// Records of this type can be viewed through the (DS 50.10.00) screen.
    /// </summary>
    public class ESignSyncProcess : PXGraph<ESignSyncProcess>
    {
        public PXCancel<ESignEnvelopeInfo> Cancel;

        [PXFilterable]
        public PXProcessingJoin<ESignEnvelopeInfo,
            InnerJoin<UploadFileRevision,
                On<UploadFileRevision.fileID, Equal<ESignEnvelopeInfo.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<ESignEnvelopeInfo.fileRevisionID>>>>,
            Where<ESignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                And<ESignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusCompleted>,
                    And<ESignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusVoided>,
                        And<ESignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusSigned>,
                            And<ESignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusDeclined>,
                                And<ESignEnvelopeInfo.isFinalVersion, NotEqual<True>>>>>>>> Envelopes;

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute), "DisplayName", Messages.ESignEnvelopeInfo.AccountOwner)]
        protected virtual void ESignEnvelopeInfo_ESignAccountID_CacheAttached(PXCache sender)
        {
        }

        public ESignSyncProcess()
        {
            Envelopes.SetSelected<ESignEnvelopeInfo.selected>();
            Envelopes.SetProcessDelegate<WikiFileMaintenance>((graph, envelope) =>
            {
                var ext = graph.GetExtension<WikiFileMaintenanceESExt>();
                var uploadGraph = CreateInstance<UploadFileMaintenance>();
                var taskGraph = CreateInstance<CRTaskMaint>();
                ext.CheckStatus(graph, envelope, uploadGraph, taskGraph);
            });
        }
    }
}
