﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YellowstonePathology.Business.Test.PDL122C3forNonsmallCellLungCancer
{
    public class PDL122C3forNonsmallCellLungCancerTest : YellowstonePathology.Business.PanelSet.Model.PanelSet
    {
        public PDL122C3forNonsmallCellLungCancerTest()
        {
            this.m_PanelSetId = 369;
            this.m_PanelSetName = "PD-L1 (22C3) for Non-small Cell Lung Cancer";
            this.m_Abbreviation = "PD-L1 (22C3)NonSmllCllLungCncr";
            this.m_CaseType = YellowstonePathology.Business.CaseType.IHC;
            this.m_HasTechnicalComponent = true;
            this.m_HasProfessionalComponent = false;
            this.m_ResultDocumentSource = PanelSet.Model.ResultDocumentSourceEnum.YPIDatabase;
            this.m_ReportNoLetter = new YellowstonePathology.Business.ReportNoLetterR();
            this.m_Active = true;
            this.m_ExpectedDuration = TimeSpan.FromDays(2);

            this.m_SurgicalAmendmentRequired = true;
            this.m_AllowMultiplePerAccession = true;

            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.WORD);
            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.EPIC);
            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.WPH);
            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.CMMC);

            this.m_PanelSetOrderClassName = typeof(YellowstonePathology.Business.Test.PDL122C3forNonsmallCellLungCancer.PDL122C3forNonsmallCellLungCancerTestOrder).AssemblyQualifiedName;
            this.m_WordDocumentClassName = typeof(YellowstonePathology.Business.Test.PDL122C3forNonsmallCellLungCancer.PDL122C3forNonsmallCellLungCancerWordDocument).AssemblyQualifiedName;

            string taskDescription = "Collect paraffin block from Histology and send to Neo.";

            YellowstonePathology.Business.Facility.Model.Facility neogenomicsIrvine = YellowstonePathology.Business.Facility.Model.FacilityCollection.Instance.GetByFacilityId("NEOGNMCIRVN");
            this.m_TaskCollection.Add(new YellowstonePathology.Business.Task.Model.TaskFedexShipment(YellowstonePathology.Business.Task.Model.TaskAssignment.Flow, taskDescription, neogenomicsIrvine));

            YellowstonePathology.Business.Billing.Model.PanelSetCptCode panelSetCptCode = new YellowstonePathology.Business.Billing.Model.PanelSetCptCode(Store.AppDataStore.Instance.CPTCodeCollection.GetClone("88360", null), 1);
            this.m_PanelSetCptCodeCollection.Add(panelSetCptCode);

            this.m_TechnicalComponentFacility = neogenomicsIrvine;
            this.m_TechnicalComponentBillingFacility = neogenomicsIrvine;

            this.m_UniversalServiceIdCollection.Add(new YellowstonePathology.Business.ClientOrder.Model.UniversalServiceDefinitions.UniversalServiceMOLEGEN());
        }
    }
}
