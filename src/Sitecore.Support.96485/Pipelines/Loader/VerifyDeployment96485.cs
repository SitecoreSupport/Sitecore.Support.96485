using Sitecore.Pipelines;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Pipelines.Loader
{
    public class VerifyDeployment96485
    {
        public void Process(PipelineArgs args) {
            try
            {
                Log.Info(GetTraceMsg("Verifying deployment of the Sitecore Support Patch"), this);
                var coreDb = Sitecore.Configuration.Factory.GetDatabase("core");
                if (coreDb == null)
                {
                    Log.Warn(GetTraceMsg("Failed to retrieve 'core' database. Aborting"), this);
                    return;
                }

                var fldDefinitionItm = coreDb.GetItem("{CD5FCA63-20B2-4715-B4BE-192B493EA87E}");
                if (fldDefinitionItm == null)
                {
                    Log.Warn(GetTraceMsg("Failed to retrieve '/sitecore/system/Field types/System Types/Query Datasource'({CD5FCA63-20B2-4715-B4BE-192B493EA87E}) item from the 'core' database. Aborting"), this);
                    return;
                } else
                {
                    Log.Info(GetTraceMsg("Retrieved '/sitecore/system/Field types/System Types/Query Datasource'({CD5FCA63-20B2-4715-B4BE-192B493EA87E}) item from the 'core' database"), this);
                }

                AssertFieldValue(fldDefinitionItm, "Assembly", "Sitecore.Support.96485");
                AssertFieldValue(fldDefinitionItm, "Class", "Sitecore.Support.Buckets.FieldTypes.BucketInternalLink");
                AssertFieldValue(fldDefinitionItm, "Control", "");
            } catch (Exception ex)
            {
                Log.Error(GetTraceMsg("Failed to verify deployment of the Sitecore Support Patch"), ex, this);
            }

        }

        private void AssertFieldValue(Sitecore.Data.Items.Item item, string fieldName, string expectedValue)
        {
            try
            {
                var actualFldValue = item[fieldName];
                if (String.Equals(actualFldValue, expectedValue, StringComparison.InvariantCulture))
                {
                    Log.Info(GetTraceMsg("The field '{0}' has expected value '{1}'".FormatWith(fieldName, expectedValue)), this);
                }
                else
                {
                    Log.Warn(GetTraceMsg("'{0}' field does not have expected value. Expected value is {1}, current value is '{2}'".FormatWith(fieldName, expectedValue, actualFldValue)), this);
                }
            }
            catch (Exception ex) {
                Log.Error(GetTraceMsg("Failed to verify '{0}' field name value due to an error."), ex, this);
            }
        }

        private string GetTraceMsg(string msg)
        {
            return "{0}:{1}".FormatWith("[#96485]", msg);
        }
    }
}