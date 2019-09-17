﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.Business.Test.HPV
{
	public class HPVTest : YellowstonePathology.Business.PanelSet.Model.PanelSetMolecularTest
	{
		public HPVTest()
		{
			this.m_PanelSetId = 14;
			this.m_PanelSetName = "High Risk HPV";
            this.m_Abbreviation = "HPV";
            this.m_CaseType = YellowstonePathology.Business.CaseType.Molecular;
			this.m_HasTechnicalComponent = true;			
			this.m_HasProfessionalComponent = false;
			this.m_ResultDocumentSource = YellowstonePathology.Business.PanelSet.Model.ResultDocumentSourceEnum.YPIDatabase;
            this.m_ReportNoLetter = new YellowstonePathology.Business.ReportNoLetterM();
            this.m_Active = true;

			this.m_PanelSetOrderClassName = typeof(YellowstonePathology.Business.Test.HPV.HPVTestOrder).AssemblyQualifiedName;
            this.m_WordDocumentClassName = typeof(YellowstonePathology.Business.Test.HPV.HPVWordDocument).AssemblyQualifiedName;                        
			this.m_AcceptOnFinal = true;
			this.m_AllowMultiplePerAccession = true;
            this.m_ExpectedDuration = TimeSpan.FromDays(4);
            this.m_AttemptOrderTargetLookup = true;
            this.m_RequiresAssignment = false;

            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.WORD);
            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.EPIC);
            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.WPH);
            this.m_ImplementedResultTypes.Add(Business.Test.ResultType.CMMC);

            this.m_AddAliquotOnOrder = true;
            this.m_AliquotToAddOnOrder = new YellowstonePathology.Business.Specimen.Model.PantherAliquot();
            this.m_SendOrderToPanther = true;

            string taskDescription = "Perform HPV Testing on sample.";
			this.m_TaskCollection.Add(new YellowstonePathology.Business.Task.Model.Task(YellowstonePathology.Business.Task.Model.TaskAssignment.Molecular, taskDescription));

            this.m_TechnicalComponentFacility = YellowstonePathology.Business.Facility.Model.FacilityCollection.Instance.GetByFacilityId("YPIBLGS");
            this.m_TechnicalComponentBillingFacility = YellowstonePathology.Business.Facility.Model.FacilityCollection.Instance.GetByFacilityId("YPIBLGS");

            this.m_HasSplitCPTCode = false;

            YellowstonePathology.Business.Billing.Model.PanelSetCptCode panelSetCptCode = new YellowstonePathology.Business.Billing.Model.PanelSetCptCode(Store.AppDataStore.Instance.CPTCodeCollection.GetClone("87624", null), 1);
            this.m_PanelSetCptCodeCollection.Add(panelSetCptCode);

            this.m_UniversalServiceIdCollection.Add(new YellowstonePathology.Business.ClientOrder.Model.UniversalServiceDefinitions.UniversalServiceHRHPVTEST());

            YellowstonePathology.Business.Specimen.Model.Specimen thinPrepFluid = YellowstonePathology.Business.Specimen.Model.SpecimenCollection.Instance.GetSpecimen("SPCMNTHNPRPFLD"); // Definition.ThinPrepFluid();
            this.OrderTargetTypeCollectionRestrictions.Add(thinPrepFluid);
		}
	}
}
