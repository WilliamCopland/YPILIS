﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YellowstonePathology.Business.Test.LynchSyndrome
{
	public class LSEColorectalResult11 : LSEResult
	{
		public LSEColorectalResult11()
		{
            this.m_Indication = LSEType.COLON;
            this.m_MLH1Result = LSEResultEnum.Intact;
			this.m_MSH2Result = LSEResultEnum.Intact;
			this.m_MSH6Result = LSEResultEnum.Loss;
			this.m_PMS2Result = LSEResultEnum.Loss;
			this.m_BrafResult = LSEResultEnum.NotPerformed;
			this.m_MethResult = LSEResultEnum.NotPerformed;
            this.m_BRAFIsIndicated = false;

			//this.m_Interpretation = "Loss of nuclear expression of PMS2 mismatch repair protein.";
			this.m_Comment = "This staining pattern is highly suggestive of Lynch Syndrome and is associated with germline PMS2 or MLH1 mutations.  " +
                "Recommend genetic counseling and further evaluation.";
            this.m_Method = IHCMethod;
            this.m_References = LSEColonReferences;
		}
	}
}
