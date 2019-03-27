// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
using System.Collections.Generic;

namespace Deploy.Plugins
{
    public class SdkMessageStepComparer : IEqualityComparer<SdkMessageProcessingStep>
    {
        public bool Equals(SdkMessageProcessingStep x, SdkMessageProcessingStep y)
        {
            return (x.SdkMessageFilterId != null && y.SdkMessageFilterId != null 
                        && (x.SdkMessageFilterId.Id == y.SdkMessageFilterId.Id))
                        && x.StageEnum == y.StageEnum && x.SdkMessageId.Id == y.SdkMessageId.Id
                        && x.ModeEnum == y.ModeEnum && x.EventHandler.Id == y.EventHandler.Id;
        }

        public int GetHashCode(SdkMessageProcessingStep obj)
        {
            return (obj.SdkMessageFilterId?.GetHashCode() ?? 0) ^ obj.Stage.GetHashCode() ^ obj.Mode.GetHashCode() ^ obj.SdkMessageId.GetHashCode() ^ obj.EventHandler.GetHashCode();
        }

        public bool NeedsUpdate(SdkMessageProcessingStep x, SdkMessageProcessingStep y)
        {
            var needsUpdate = x.AsyncAutoDelete != y.AsyncAutoDelete;
            needsUpdate |= x.Configuration != y.Configuration;
            needsUpdate |= x.FilteringAttributes != y.FilteringAttributes;
            needsUpdate |= !((x.ImpersonatingUserId == null && y.ImpersonatingUserId == null)
                                || (x.ImpersonatingUserId != null && y.ImpersonatingUserId != null)
                                || (x.ImpersonatingUserId.Id == y.ImpersonatingUserId.Id));
            needsUpdate |= x.Rank != y.Rank;
            needsUpdate |= x.Description != y.Description;

            return needsUpdate;
        }
    }
}
