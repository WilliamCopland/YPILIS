﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace YellowstonePathology.Business.Test.CalreticulinMutationAnalysis
{
	public class CalreticulinMutationAnalysisEPICObxView : YellowstonePathology.Business.HL7View.EPIC.EPICObxView
	{
		public CalreticulinMutationAnalysisEPICObxView(YellowstonePathology.Business.Test.AccessionOrder accessionOrder, string reportNo, int obxCount)
			: base(accessionOrder, reportNo, obxCount)
		{
		}

		public override void ToXml(XElement document)
		{
			CalreticulinMutationAnalysisTestOrder panelSetOrder = (CalreticulinMutationAnalysisTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(this.m_ReportNo);
			this.AddHeader(document, panelSetOrder, "Calreticulin Mutation Analysis");

			this.AddNextObxElement("", document, "F");
			string result = "Result: " + panelSetOrder.Result;
            if(result == "Detected")
            {
                result = result + "(" + panelSetOrder.Mutations + ")";
            }

			this.AddNextObxElement(result, document, "F");

			this.AddNextObxElement("", document, "F");
			this.AddNextObxElement("Pathologist: " + panelSetOrder.Signature, document, "F");
			if (panelSetOrder.FinalTime.HasValue == true)
			{
				this.AddNextObxElement("E-signed " + panelSetOrder.FinalTime.Value.ToString("MM/dd/yyyy HH:mm"), document, "F");
			}

			this.AddNextObxElement("", document, "F");
            this.AddAmendments(document);

            this.AddNextObxElement("Specimen Information:", document, "F");
			YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetSpecimenOrder(panelSetOrder.OrderedOn, panelSetOrder.OrderedOnId);
			this.AddNextObxElement("Specimen Identification: " + specimenOrder.Description, document, "F");
			string collectionDateTimeString = YellowstonePathology.Business.Helper.DateTimeExtensions.CombineDateAndTime(specimenOrder.CollectionDate, specimenOrder.CollectionTime);
			this.AddNextObxElement("Collection Date/Time: " + collectionDateTimeString, document, "F");

			this.AddNextObxElement("", document, "F");
			this.AddNextObxElement("Interpretation:", document, "F");
			this.HandleLongString(panelSetOrder.Interpretation, document, "F");

			this.AddNextObxElement("", document, "F");
			this.AddNextObxElement("Method:", document, "F");
			this.HandleLongString(panelSetOrder.Method, document, "F");

			this.AddNextObxElement("", document, "F");
			this.AddNextObxElement("References:", document, "F");
			this.HandleLongString(panelSetOrder.ReportReferences, document, "F");

			this.AddNextObxElement("This test was performed using a US FDA approved DNA probe kit.  The FDA procedure was performed using a modified DNA extraction method for test optimization, and the modified procedure was validated by Yellowstone Pathology Institute (YPI).  YPI assumes the responsibility for test performance. Laboratory Improvement Amendments of 1988 (CLIA-88) as qualified to perform high complexity clinical laboratory testing.", document, "F");
            this.AddNextObxElement("", document, "F");
            this.AddNextObxElement(panelSetOrder.ASR, document, "F");

            this.AddNextObxElement("", document, "F");
            string locationPerformed = panelSetOrder.GetLocationPerformedComment();
			this.AddNextObxElement(locationPerformed, document, "F");
			this.AddNextObxElement(string.Empty, document, "F");
		}
	}
}
