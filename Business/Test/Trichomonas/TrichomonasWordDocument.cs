using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace YellowstonePathology.Business.Test.Trichomonas
{
	public class TrichomonasWordDocument : YellowstonePathology.Business.Document.CaseReportV2
    {
		public override void Render(string masterAccessionNo, string reportNo, YellowstonePathology.Business.Document.ReportSaveModeEnum reportSaveEnum, object writer)
		{
            this.m_ReportNo = reportNo;
            this.m_ReportNo = reportNo;
			this.m_ReportSaveEnum = reportSaveEnum;

			this.m_AccessionOrder = YellowstonePathology.Business.Persistence.DocumentGateway.Instance.PullAccessionOrder(masterAccessionNo, writer);
			TrichomonasTestOrder reportOrderTrichomonas = (TrichomonasTestOrder)this.m_AccessionOrder.PanelSetOrderCollection.GetPanelSetOrder(reportNo);
            this.m_PanelSetOrder = reportOrderTrichomonas;

			this.m_TemplateName = @"\\CFileServer\Documents\ReportTemplates\XmlTemplates\Trichomonas.5.xml";
			base.OpenTemplate();

            this.SetDemographicsV2();
            this.SetReportDistribution();
            this.SetCaseHistory();

			YellowstonePathology.Business.Document.AmendmentSection amendmentSection = new YellowstonePathology.Business.Document.AmendmentSection();
            amendmentSection.SetAmendment(reportOrderTrichomonas.AmendmentCollection, this.m_ReportXml, this.m_NameSpaceManager, false);

            this.SetXmlNodeData("test_result", reportOrderTrichomonas.Result);
            this.SetXmlNodeData("test_method", reportOrderTrichomonas.Method);

			this.SetXmlNodeData("final_date", YellowstonePathology.Business.BaseData.GetShortDateString(this.m_PanelSetOrder.FinalDate));

			YellowstonePathology.Business.Specimen.Model.SpecimenOrder specimenOrder = this.m_AccessionOrder.SpecimenOrderCollection.GetSpecimenOrder(this.m_PanelSetOrder.OrderedOn, this.m_PanelSetOrder.OrderedOnId);
            base.ReplaceText("specimen_description", specimenOrder.Description);

            string collectionDateTimeString = YellowstonePathology.Business.Helper.DateTimeExtensions.CombineDateAndTime(specimenOrder.CollectionDate, specimenOrder.CollectionTime);
            this.SetXmlNodeData("date_time_collected", collectionDateTimeString);

            this.SaveReport();
        }

        public override void Publish()
        {
            base.Publish();
        }
    }
}
